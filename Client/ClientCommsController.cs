using WebSocketSharp;
using Newtonsoft.Json;
using Shared;
using Client.Interfaces;

namespace Client
{
    public class ClientCommsController : IClientCommsController
    {
        private WebSocket? ws;
        private string username;
        private readonly ClientMessageDispatcher messageDispatcher;
        private readonly IServerConnectionProvider connectionProvider;

        public event Action<CreateRoomResponse>? OnCreateRoomResponse;
        public event Action<ChatMessage>? OnChatMessage;
        public event Action<RoomListResponse>? OnRoomListResponse;
        public event Action<JoinRoomResponse>? OnJoinRoomResponse;
        public event Action<AddContactResponse>? OnAddContactResponse;
        public event Action<ContactListResponse>? OnContactListResponse;
        public event Action<string>? OnError;
        public event Action? OnDisconnect;

        public ClientCommsController(string username, IServerConnectionProvider connectionProvider)
        {
            this.username = username;
            this.connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            this.messageDispatcher = new ClientMessageDispatcher();
            RegisterMessageHandlers();
        }

        private void RegisterMessageHandlers()
        {
            messageDispatcher.RegisterHandler<CreateRoomResponse>("CREATE_ROOM_RESPONSE",
                response => OnCreateRoomResponse?.Invoke(response));
            messageDispatcher.RegisterHandler<ChatMessage>("CHAT",
                chatMsg => OnChatMessage?.Invoke(chatMsg));
            messageDispatcher.RegisterHandler<RoomListResponse>("ROOM_LIST",
                roomList => OnRoomListResponse?.Invoke(roomList));
            messageDispatcher.RegisterHandler<JoinRoomResponse>("JOIN_ROOM_RESPONSE",
                joinResponse => OnJoinRoomResponse?.Invoke(joinResponse));
            messageDispatcher.RegisterHandler<AddContactResponse>("ADD_CONTACT_RESPONSE",
                addContactResponse => OnAddContactResponse?.Invoke(addContactResponse));
            messageDispatcher.RegisterHandler<ContactListResponse>("CONTACT_LIST",
                contactList => OnContactListResponse?.Invoke(contactList));
        }

        public void ConnectToServer()
        {
            ws = new WebSocket(connectionProvider.GetServerUrl());

            ws.OnOpen += (sender, e) =>
            {
                var loginMsg = new LoginMessage { Username = username };
                var msg = new Shared.Message("LOGIN", JsonConvert.SerializeObject(loginMsg));
                ws.Send(JsonConvert.SerializeObject(msg));
            };

            ws.OnMessage += (sender, e) =>
            {
                var msg = JsonConvert.DeserializeObject<Shared.Message>(e.Data);
                if (msg != null)
                {
                    HandleMessage(msg);
                }
            };

            ws.OnError += (sender, e) =>
            {
                OnError?.Invoke($"Error: {e.Message}");
            };

            ws.OnClose += (sender, e) =>
            {
                OnDisconnect?.Invoke();
            };

            ws.Connect();
        }

        private void HandleMessage(Shared.Message msg)
        {
            messageDispatcher.Dispatch(msg);
        }

        public void SendCreateRoom(string roomName, string password)
        {
            var request = new CreateRoomRequest
            {
                RoomName = roomName,
                Password = password,
                Username = username
            };

            var msg = new Shared.Message("CREATE_ROOM", JsonConvert.SerializeObject(request));
            ws?.Send(JsonConvert.SerializeObject(msg));
        }

        public void SendJoinRoom(string roomName, string password)
        {
            var request = new JoinRoomRequest
            {
                RoomName = roomName,
                Password = password,
                Username = username
            };

            var msg = new Shared.Message("JOIN_ROOM", JsonConvert.SerializeObject(request));
            ws?.Send(JsonConvert.SerializeObject(msg));
        }

        public void SendChatMessage(string roomName, string text)
        {
            var chatMsg = new ChatMessage
            {
                RoomName = roomName,
                Username = username,
                Text = text,
                Timestamp = DateTime.Now
            };

            var msg = new Shared.Message("CHAT", JsonConvert.SerializeObject(chatMsg));
            ws?.Send(JsonConvert.SerializeObject(msg));
        }

        public void SendAddContact(string contactUsername)
        {
            var request = new AddContactRequest
            {
                RequesterUsername = username,
                ContactUsername = contactUsername
            };

            var msg = new Shared.Message("ADD_CONTACT", JsonConvert.SerializeObject(request));
            ws?.Send(JsonConvert.SerializeObject(msg));
        }

        public void SendRemoveContact(string contactUsername)
        {
            var request = new RemoveContactRequest
            {
                RequesterUsername = username,
                ContactUsername = contactUsername
            };

            var msg = new Shared.Message("REMOVE_CONTACT", JsonConvert.SerializeObject(request));
            ws?.Send(JsonConvert.SerializeObject(msg));
        }

        public void Disconnect()
        {
            if (ws != null && ws.IsAlive)
            {
                try
                {
                    ws.CloseAsync(WebSocketSharp.CloseStatusCode.Normal, "Client closing");
                }
                catch
                {
                    // Ignore exceptions during close
                }
            }
        }
    }
}
