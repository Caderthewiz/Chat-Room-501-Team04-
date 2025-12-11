namespace Shared
{
    public class Message
    {
        public string Type { get; set; }
        public string Data { get; set; }

        public Message(string type, string data)
        {
            Type = type;
            Data = data;
        }
    }

    public class CreateRoomRequest
    {
        public string RoomName { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }

    public class CreateRoomResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string RoomName { get; set; }
        public string Password { get; set; }
        public List<string> Members { get; set; }
    }

    public class ChatMessage
    {
        public string RoomName { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class RoomListResponse
    {
        public List<string> RoomNames { get; set; }
    }

    public class LoginMessage
    {
        public string Username { get; set; }
    }

    public class JoinRoomRequest
    {
        public string RoomName { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }

    public class JoinRoomResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string RoomName { get; set; }
        public bool RequiresPassword { get; set; }
    }

    public class AddContactRequest
    {
        public string RequesterUsername { get; set; }
        public string ContactUsername { get; set; }
    }

    public class AddContactResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ContactUsername { get; set; }
    }

    public class ContactInfo
    {
        public string Username { get; set; }
        public bool IsOnline { get; set; }
    }

    public class ContactListResponse
    {
        public List<string> Contacts { get; set; }
        public List<ContactInfo> ContactsWithStatus { get; set; }
    }

    public class RemoveContactRequest
    {
        public string RequesterUsername { get; set; }
        public string ContactUsername { get; set; }
    }
}
