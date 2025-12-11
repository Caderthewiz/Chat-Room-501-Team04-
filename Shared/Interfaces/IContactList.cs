namespace Shared.Interfaces
{
    public interface IContactList
    {
        void AddContact(string username);
        void RemoveContact(string username);
        Contact? GetContact(string username);
        List<Contact> GetAllContacts();
        bool Contains(string username);
        int Count { get; }
    }
}
