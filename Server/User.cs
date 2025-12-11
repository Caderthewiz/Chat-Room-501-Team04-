namespace Server
{
    public class User
    {
        public string Username { get; set; }
        public bool IsOnline { get; set; }
        public string CurrentRoom { get; set; }
        public bool IsAdmin { get; set; }

        public User(string username, bool isAdmin = false)
        {
            Username = username;
            IsOnline = false;
            CurrentRoom = string.Empty;
            IsAdmin = isAdmin;
        }
    }
}
