using Shared;

namespace Client.Interfaces
{
    public interface IContactListProxy
    {
        event Action? OnContactListChanged;

        void UpdateContacts(List<string> newContacts);
        void UpdateContactsWithStatus(List<ContactInfo> contactsWithStatus);
        void AddContact(string contactUsername);
        void RemoveContact(string contactUsername);
        List<string> GetContacts();
        List<ContactInfo> GetContactsWithStatus();
        bool IsContact(string username);
        bool WasContactRemoved(string currentContact, List<string> newContacts);
    }
}
