using Shared.Interfaces;

namespace Shared
{
    public class RemoteContactList : IContactList
    {
        private readonly Dictionary<string, Contact> _contacts;

        public int Count => _contacts.Count;

        // Property to get contacts as tuples for compatibility
        public List<(string, bool)> Contacts
        {
            get
            {
                return _contacts.Values
                    .Select(c => (c.Name, c.IsOnline))
                    .ToList();
            }
        }

        public RemoteContactList()
        {
            _contacts = new Dictionary<string, Contact>();
        }

        // Overload that takes both name and status, returns bool
        public bool AddContact(string username, bool isOnline)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            if (_contacts.ContainsKey(username))
            {
                // Update status if contact exists
                _contacts[username].IsOnline = isOnline;
                return true;
            }

            _contacts[username] = new Contact(username, isOnline);
            return true;
        }

        public void AddContact(string username)
        {
            AddContact(username, false);
        }

        // Explicit interface implementation
        void IContactList.RemoveContact(string username)
        {
            _contacts.Remove(username);
        }

        // Public method with bool return for convenience
        public bool RemoveContact(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            return _contacts.Remove(username);
        }

        public Contact? GetContact(string username)
        {
            return _contacts.ContainsKey(username) ? _contacts[username] : null;
        }

        public List<Contact> GetAllContacts()
        {
            return _contacts.Values.ToList();
        }

        public bool Contains(string username)
        {
            return _contacts.ContainsKey(username);
        }

        public void UpdateContactStatus(string username, bool isOnline)
        {
            if (_contacts.ContainsKey(username))
            {
                _contacts[username].IsOnline = isOnline;
            }
        }

        public void UpdateFromContactInfo(List<ContactInfo> contactsWithStatus)
        {
            _contacts.Clear();
            foreach (var contactInfo in contactsWithStatus)
            {
                _contacts[contactInfo.Username] = new Contact(contactInfo.Username, contactInfo.IsOnline);
            }
        }
    }
}
