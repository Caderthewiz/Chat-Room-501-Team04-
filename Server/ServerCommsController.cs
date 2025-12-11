using WebSocketSharp.Server;
using Newtonsoft.Json;
using Shared;
using Server.Interfaces;

namespace Server
{
    public class ServerCommsController : IServerCommsController
    {
        private WebSocketServer? server;
        private readonly IServerController serverController;
        private Action<string> logCallback;
        private readonly MessageDispatcher messageDispatcher;

        public ServerCommsController(IServerController controller, Action<string> logCallback)
        {
            this.serverController = controller ?? throw new ArgumentNullException(nameof(controller));
            this.logCallback = logCallback ?? throw new ArgumentNullException(nameof(logCallback));
            this.messageDispatcher = new MessageDispatcher();

            // Register message handlers
            RegisterMessageHandlers();

            // Subscribe to room list changes to broadcast updates
            serverController.OnRoomListChanged += BroadcastRoomList;
            serverController.OnUserStatusChanged += BroadcastContactListUpdates;
        }

        private void RegisterMessageHandlers()
        {
            messageDispatcher.RegisterHandler<LoginMessage>("LOGIN", HandleLogin);
            messageDispatcher.RegisterHandler<CreateRoomRequest>("CREATE_ROOM", HandleCreateRoom);
            messageDispatcher.RegisterHandler<JoinRoomRequest>("JOIN_ROOM", HandleJoinRoom);
            messageDispatcher.RegisterHandler<AddContactRequest>("ADD_CONTACT", HandleAddContact);
            messageDispatcher.RegisterHandler<RemoveContactRequest>("REMOVE_CONTACT", HandleRemoveContact);
            messageDispatcher.RegisterHandler<ChatMessage>("CHAT", HandleChatMessage);
        }

        public MessageDispatcher GetMessageDispatcher() => messageDispatcher;

        public void StartServer()
        {
            server = new WebSocketServer("ws://127.0.0.1:8080");
            server.AddWebSocketService<ChatServerBehavior>("/login", (behavior) =>
            {
                behavior.ServerCommsController = this;
            });
            server.Start();
            logCallback("Server started on ws://127.0.0.1:8080/login");
        }

        private void BroadcastRoomList()
        {
            if (server != null && server.IsListening)
            {
                var roomList = serverController.GetRoomList();
                server.WebSocketServices["/login"].Sessions.Broadcast(
                    JsonConvert.SerializeObject(new Shared.Message("ROOM_LIST", JsonConvert.SerializeObject(roomList)))
                );
            }
        }

        private void BroadcastContactListUpdates(string username)
        {
            if (server == null || !server.IsListening) return;

            // Get all users who have this user as a contact
            var usersToNotify = serverController.GetUsersWhoHaveContact(username);

            foreach (var userToNotify in usersToNotify)
            {
                // Find the session for this user
                var session = server.WebSocketServices["/login"].Sessions.Sessions
                    .FirstOrDefault(s => ((IClientSession)s).Username == userToNotify);

                if (session != null)
                {
                    SendContactListToClient((IClientSession)session, userToNotify);
                }
            }
        }

        public void StopServer()
        {
            if (server != null && server.IsListening)
            {
                server.Stop();
                logCallback("Server stopped");
            }
        }

        public void HandleLogin(LoginMessage loginMsg, IClientSession sender)
        {
            sender.Username = loginMsg.Username;

            // Check if user exists in server's user repository, if not, add them
            // This handles the case where a new user was created while server was running
            serverController.EnsureUserExists(loginMsg.Username);

            serverController.SetUserOnline(loginMsg.Username, true);
            SendContactListToClient(sender, loginMsg.Username);
        }

        public void HandleCreateRoom(CreateRoomRequest request, IClientSession sender)
        {
            var response = serverController.CreateRoom(request);

            // Broadcast to all clients
            if (server != null && server.IsListening)
            {
                server.WebSocketServices["/login"].Sessions.Broadcast(
                    JsonConvert.SerializeObject(new Shared.Message("CREATE_ROOM_RESPONSE", JsonConvert.SerializeObject(response)))
                );
            }
        }

        public void HandleJoinRoom(JoinRoomRequest request, IClientSession sender)
        {
            string currentRoom = sender.CurrentRoom ?? string.Empty;
            var response = serverController.JoinRoom(request, ref currentRoom);
            sender.CurrentRoom = currentRoom;

            sender.SendMessage("JOIN_ROOM_RESPONSE", response);

            // Send chat history if join was successful
            if (response.Success)
            {
                var history = serverController.GetChatHistory(request.RoomName);
                foreach (var historicalMsg in history)
                {
                    sender.SendMessage("CHAT", historicalMsg);
                }
            }
        }

        public void HandleChatMessage(ChatMessage chatMsg, IClientSession sender)
        {
            serverController.HandleChatMessage(chatMsg);

            // Broadcast to all clients
            if (server != null && server.IsListening)
            {
                server.WebSocketServices["/login"].Sessions.Broadcast(
                    JsonConvert.SerializeObject(new Shared.Message("CHAT", JsonConvert.SerializeObject(chatMsg)))
                );
            }
        }

        public void HandleAddContact(AddContactRequest request, IClientSession sender)
        {
            var response = serverController.AddContact(request);
            sender.SendMessage("ADD_CONTACT_RESPONSE", response);

            // Send updated contact lists to both users if successful
            if (response.Success && server != null)
            {
                // Send to requester
                SendContactListToClient(sender, request.RequesterUsername);

                // Send to the added contact if they're online
                var contactSession = server.WebSocketServices["/login"].Sessions.Sessions
                    .FirstOrDefault(s => ((IClientSession)s).Username == request.ContactUsername);

                if (contactSession != null)
                {
                    SendContactListToClient((IClientSession)contactSession, request.ContactUsername);
                }
            }
        }

        public void HandleRemoveContact(RemoveContactRequest request, IClientSession sender)
        {
            serverController.RemoveContact(request);

            // Send updated contact lists
            SendContactListToClient(sender, request.RequesterUsername);

            // Send to removed contact if online
            if (server != null)
            {
                var contactSession = server.WebSocketServices["/login"].Sessions.Sessions
                    .FirstOrDefault(s => ((IClientSession)s).Username == request.ContactUsername);

                if (contactSession != null)
                {
                    SendContactListToClient((IClientSession)contactSession, request.ContactUsername);
                }
            }
        }

        public void HandleDisconnect(string username)
        {
            serverController.SetUserOnline(username, false);
            serverController.RemoveUserFromAllRooms(username);
            // Room list broadcast handled automatically by OnRoomListChanged event
        }

        public void SendRoomListToClient(IClientSession client)
        {
            var response = serverController.GetRoomList();
            client.SendMessage("ROOM_LIST", response);
            logCallback($"Sent room list to client: {client.Username}");
        }

        public void SendContactListToClient(IClientSession client, string username)
        {
            var response = serverController.GetContactList(username);
            client.SendMessage("CONTACT_LIST", response);
            logCallback($"Sent contact list to {username}");
        }
    }
}
