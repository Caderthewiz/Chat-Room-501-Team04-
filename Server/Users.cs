using Server.Interfaces;

namespace Server
{
    public class Users : IUserRepository
    {
        private readonly Dictionary<string, User> _users;

        public int OnlineCount => _users.Values.Count(u => u.IsOnline);
        public int OfflineCount => _users.Values.Count(u => !u.IsOnline);

        public Users()
        {
            _users = new Dictionary<string, User>();
        }

        public void AddUser(User user)
        {
            if (!_users.ContainsKey(user.Username))
            {
                _users[user.Username] = user;
            }
        }

        public void RemoveUser(string username)
        {
            _users.Remove(username);
        }

        public User? GetUser(string username)
        {
            return _users.ContainsKey(username) ? _users[username] : null;
        }

        public bool UserExists(string username)
        {
            return _users.ContainsKey(username);
        }

        public void SetUserOnline(string username, bool isOnline)
        {
            if (_users.ContainsKey(username))
            {
                _users[username].IsOnline = isOnline;
            }
        }

        public void SetUserRoom(string username, string roomName)
        {
            if (_users.ContainsKey(username))
            {
                _users[username].CurrentRoom = roomName;
            }
        }

        public List<string> GetOnlineUsers()
        {
            return _users.Values.Where(u => u.IsOnline).Select(u => u.Username).ToList();
        }

        public List<string> GetOfflineUsers()
        {
            return _users.Values.Where(u => !u.IsOnline).Select(u => u.Username).ToList();
        }

        public List<string> GetAllUsernames()
        {
            return _users.Keys.ToList();
        }

        // UML-aligned method names (aliases for compatibility)
        public List<User> GetClients()
        {
            return _users.Values.Where(u => !u.IsAdmin).ToList();
        }

        public List<User> GetAdmins()
        {
            return _users.Values.Where(u => u.IsAdmin).ToList();
        }

        public List<User> GetOnlineClients()
        {
            return _users.Values.Where(u => !u.IsAdmin && u.IsOnline).ToList();
        }

        public List<User> GetOfflineClients()
        {
            return _users.Values.Where(u => !u.IsAdmin && !u.IsOnline).ToList();
        }
    }
}
