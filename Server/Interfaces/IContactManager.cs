using Shared;

namespace Server.Interfaces
{
    public interface IContactManager
    {
        ContactListResponse GetContactList(string username);
        AddContactResponse AddContact(AddContactRequest request);
        void RemoveContact(RemoveContactRequest request);
        List<string> GetUsersWhoHaveContact(string username);
    }
}
