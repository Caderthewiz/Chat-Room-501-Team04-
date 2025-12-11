namespace Server.Interfaces
{
    public interface IUserRepository
    {
        void AddUser(User user);
        User? GetUser(string username);
        void SetUserOnline(string username, bool isOnline);
        void SetUserRoom(string username, string roomName);
        List<string> GetOnlineUsers();
        List<string> GetOfflineUsers();
        int OnlineCount { get; }
        int OfflineCount { get; }
    }
}
