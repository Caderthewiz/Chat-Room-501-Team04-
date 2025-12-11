namespace Shared.Interfaces
{
    public interface IContactRepository
    {
        bool AddContact(string username, string contactUsername);
        bool RemoveContact(string username, string contactUsername);
        List<string> GetContacts(string username);
        bool AreContacts(string username1, string username2);
        List<string> GetUsersWhoHaveContact(string username);
    }
}
