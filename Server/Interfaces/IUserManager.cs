namespace Server.Interfaces
{
    public interface IUserManager
    {
        void EnsureUserExists(string username);
        void SetUserOnline(string username, bool isOnline);
        void RemoveUserFromAllRooms(string username);
        List<string> GetOnlineUsers();
        List<string> GetOfflineUsers();
    }
}
