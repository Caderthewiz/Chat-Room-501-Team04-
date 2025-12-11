using Newtonsoft.Json;
using Shared;
using Shared.Interfaces;
using Server.Interfaces;

namespace Server
{
    public class ServerController : IServerController
    {
        private readonly List<Room> rooms;
        private readonly IUserRepository users;
        private readonly IContactRepository contactsManager;
        private readonly ICredentialRepository credentialsM;
        private readonly IChatHistoryRepository chatHistory;
        private readonly IRoomTypeClassifier roomTypeClassifier;

        public event Action<string>? OnLogUpdate;
        public event Action? OnRoomListChanged;
        public event Action? OnUserListChanged;
        public event Action<string>? OnUserStatusChanged;

        public ServerController(
            IUserRepository userRepository,
            IContactRepository contactRepository,
            ICredentialRepository credentialRepository,
            IChatHistoryRepository chatHistoryRepository,
            IRoomTypeClassifier roomTypeClassifier)
        {
            this.users = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.contactsManager = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
            this.credentialsM = credentialRepository ?? throw new ArgumentNullException(nameof(credentialRepository));
            this.chatHistory = chatHistoryRepository ?? throw new ArgumentNullException(nameof(chatHistoryRepository));
            this.roomTypeClassifier = roomTypeClassifier ?? throw new ArgumentNullException(nameof(roomTypeClassifier));
            this.rooms = new List<Room>();

            LoadAllUsersFromCredentials();
        }

        private void LoadAllUsersFromCredentials()
        {
            var allUsernames = credentialsM.GetAllUsernames();
            foreach (var username in allUsernames)
            {
                var isAdmin = credentialsM.IsUserAdmin(username);
                User user = new User(username, isAdmin);
                users.AddUser(user);
            }
            Log($"Loaded {allUsernames.Count} users from credentials");
        }

        public CreateRoomResponse CreateRoom(CreateRoomRequest request)
        {
            Room? existingRoom = rooms.Find(r => r.Name == request.RoomName);

            if (existingRoom != null)
            {
                Log($"Room creation failed: {request.RoomName} already exists");
                return new CreateRoomResponse
                {
                    Success = false,
                    Message = "Room already exists"
                };
            }

            Room newRoom = new Room(request.RoomName, request.Password);
            rooms.Add(newRoom);

            Log($"Room created: {request.RoomName} by {request.Username}");
            OnRoomListChanged?.Invoke();

            return new CreateRoomResponse
            {
                Success = true,
                Message = "Room created successfully",
                RoomName = newRoom.Name,
                Password = newRoom.Password,
                Members = new List<string>(newRoom.Clients)
            };
        }

        public JoinRoomResponse JoinRoom(JoinRoomRequest request, ref string userCurrentRoom)
        {
            // If user is already in this room, just return success (no need to rejoin)
            if (userCurrentRoom == request.RoomName)
            {
                Log($"{request.Username} already in room: {request.RoomName}");
                return new JoinRoomResponse
                {
                    Success = true,
                    Message = "Already in room",
                    RoomName = request.RoomName,
                    RequiresPassword = false
                };
            }

            // Remove user from their current room first
            if (!string.IsNullOrEmpty(userCurrentRoom))
            {
                LeaveRoom(userCurrentRoom, request.Username);
            }

            Room? targetRoom = rooms.Find(r => r.Name == request.RoomName);

            // Auto-create DM rooms if they don't exist
            if (targetRoom == null && roomTypeClassifier.IsDirectMessage(request.RoomName))
            {
                targetRoom = new Room(request.RoomName, "");
                rooms.Add(targetRoom);
                Log($"DM room auto-created: {request.RoomName}");
            }

            if (targetRoom == null)
            {
                return new JoinRoomResponse
                {
                    Success = false,
                    Message = "Room not found",
                    RoomName = request.RoomName,
                    RequiresPassword = false
                };
            }

            // Check password
            if (!string.IsNullOrEmpty(targetRoom.Password))
            {
                if (string.IsNullOrEmpty(request.Password))
                {
                    Log($"Password required for {request.Username} to join {request.RoomName}");
                    return new JoinRoomResponse
                    {
                        Success = false,
                        Message = "Password required",
                        RoomName = request.RoomName,
                        RequiresPassword = true
                    };
                }
                else if (request.Password != targetRoom.Password)
                {
                    Log($"Failed join attempt by {request.Username} to {request.RoomName}: incorrect password");
                    return new JoinRoomResponse
                    {
                        Success = false,
                        Message = "Incorrect password",
                        RoomName = request.RoomName,
                        RequiresPassword = true
                    };
                }
            }

            // Add user to room
            if (!targetRoom.Clients.Contains(request.Username))
            {
                targetRoom.AddMember(request.Username);
                OnRoomListChanged?.Invoke();
            }

            // Update user's current room
            userCurrentRoom = request.RoomName;
            users.SetUserRoom(request.Username, request.RoomName);

            Log($"{request.Username} joined room: {request.RoomName}");

            return new JoinRoomResponse
            {
                Success = true,
                Message = "Joined room successfully",
                RoomName = request.RoomName,
                RequiresPassword = false
            };
        }

        public void LeaveRoom(string roomName, string username)
        {
            Room? room = rooms.Find(r => r.Name == roomName);
            if (room == null) return;

            room.RemoveMember(username);

            if (room.Clients.Count == 0)
            {
                rooms.Remove(room);

                // Clear chat history for deleted room (but preserve DM history)
                if (roomTypeClassifier.IsPublicRoom(roomName) && chatHistory.HasHistory(roomName))
                {
                    chatHistory.ClearHistory(roomName);
                    Log($"Cleared chat history for deleted room: {roomName}");
                }
                else if (roomTypeClassifier.IsDirectMessage(roomName))
                {
                    Log($"DM room deleted but history preserved: {roomName}");
                }
                else
                {
                    Log($"No chat history found for room: {roomName}");
                }

                Log($"Room deleted (empty): {roomName}");
                OnRoomListChanged?.Invoke();
            }
        }

        public void HandleChatMessage(ChatMessage chatMsg)
        {
            chatHistory.AddMessage(chatMsg.RoomName, chatMsg);
            Log($"[{chatMsg.RoomName}] {chatMsg.Username}: {chatMsg.Text}");
        }

        public List<ChatMessage> GetChatHistory(string roomName)
        {
            return chatHistory.GetHistory(roomName);
        }

        public RoomListResponse GetRoomList()
        {
            var roomNames = rooms.Where(r => roomTypeClassifier.IsPublicRoom(r.Name)).Select(r => r.Name).ToList();
            return new RoomListResponse { RoomNames = roomNames };
        }

        public ContactListResponse GetContactList(string username)
        {
            var contacts = contactsManager.GetContacts(username);
            var contactsWithStatus = new List<ContactInfo>();

            foreach (var contactUsername in contacts)
            {
                var user = users.GetUser(contactUsername);
                contactsWithStatus.Add(new ContactInfo
                {
                    Username = contactUsername,
                    IsOnline = user?.IsOnline ?? false
                });
            }

            return new ContactListResponse
            {
                Contacts = contacts,
                ContactsWithStatus = contactsWithStatus
            };
        }

        public AddContactResponse AddContact(AddContactRequest request)
        {
            // Reload credentials to pick up any users created while server is running
            credentialsM.ReloadCredentials();

            if (!credentialsM.UserExists(request.ContactUsername))
            {
                Log($"Add contact failed: {request.ContactUsername} does not exist");
                return new AddContactResponse
                {
                    Success = false,
                    Message = "User does not exist",
                    ContactUsername = request.ContactUsername
                };
            }

            bool added1 = contactsManager.AddContact(request.RequesterUsername, request.ContactUsername);
            bool added2 = contactsManager.AddContact(request.ContactUsername, request.RequesterUsername);

            if (added1 || added2)
            {
                Log($"Contact added: {request.RequesterUsername} <-> {request.ContactUsername}");
                return new AddContactResponse
                {
                    Success = true,
                    Message = "Contact added successfully",
                    ContactUsername = request.ContactUsername
                };
            }
            else
            {
                Log($"Add contact failed: already exists");
                return new AddContactResponse
                {
                    Success = false,
                    Message = "Contact already added",
                    ContactUsername = request.ContactUsername
                };
            }
        }

        public void RemoveContact(RemoveContactRequest request)
        {
            contactsManager.RemoveContact(request.RequesterUsername, request.ContactUsername);
            contactsManager.RemoveContact(request.ContactUsername, request.RequesterUsername);
            Log($"Contact removed: {request.RequesterUsername} <-> {request.ContactUsername}");
        }

        public void EnsureUserExists(string username)
        {
            // Check if user exists in our user repository
            if (users.GetUser(username) == null)
            {
                // User doesn't exist in memory, but might exist in credentials file
                // This happens when a new user is created while the server is running
                var isAdmin = credentialsM.IsUserAdmin(username);
                User user = new User(username, isAdmin);
                users.AddUser(user);
                Log($"Added new user to repository: {username} (Admin: {isAdmin})");
                OnUserListChanged?.Invoke();
            }
        }

        public void SetUserOnline(string username, bool isOnline)
        {
            users.SetUserOnline(username, isOnline);
            OnUserListChanged?.Invoke();
            OnUserStatusChanged?.Invoke(username);
            Log($"User {(isOnline ? "online" : "offline")}: {username}");
        }

        public void RemoveUserFromAllRooms(string username)
        {
            List<Room> roomsToRemove = new List<Room>();

            foreach (var room in rooms)
            {
                room.RemoveMember(username);
                if (room.Clients.Count == 0)
                {
                    roomsToRemove.Add(room);
                }
            }

            foreach (var room in roomsToRemove)
            {
                rooms.Remove(room);

                // Clear chat history for deleted room (but preserve DM history)
                if (roomTypeClassifier.IsPublicRoom(room.Name) && chatHistory.HasHistory(room.Name))
                {
                    chatHistory.ClearHistory(room.Name);
                    Log($"Cleared chat history for deleted room: {room.Name}");
                }
                else if (roomTypeClassifier.IsDirectMessage(room.Name))
                {
                    Log($"DM room deleted but history preserved: {room.Name}");
                }

                Log($"Room deleted (empty): {room.Name}");
            }

            if (roomsToRemove.Count > 0)
            {
                OnRoomListChanged?.Invoke();
            }
        }

        public List<string> GetOnlineUsers() => users.GetOnlineUsers();
        public List<string> GetOfflineUsers() => users.GetOfflineUsers();
        public List<Room> GetAllRooms() => rooms;

        public List<string> GetUsersWhoHaveContact(string username)
        {
            return contactsManager.GetUsersWhoHaveContact(username);
        }

        private void Log(string message)
        {
            OnLogUpdate?.Invoke(message);
        }
    }
}
