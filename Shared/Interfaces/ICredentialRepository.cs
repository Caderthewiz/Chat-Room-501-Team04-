namespace Shared.Interfaces
{
    public interface ICredentialRepository
    {
        bool ValidateUser(string username, string password);
        bool CreateUser(string username, string password, bool isAdmin);
        bool UserExists(string username);
        bool IsUserAdmin(string username);
        List<string> GetAllUsernames();
        void ReloadCredentials();
    }
}
