using Shared;
using Client.Interfaces;

namespace Client
{
    public class ClientProxy : IContactListProxy
    {
        private List<Contact> _contacts;
        private Rooms rooms;

        public List<Contact> Contacts => _contacts;
        public int RoomCount => rooms?.Count ?? 0;
        public string? Username { get; set; }
        public Rooms? Rooms => rooms;

        public event Action? OnContactListChanged;

        public ClientProxy()
        {
            _contacts = new List<Contact>();
            rooms = new Rooms();
        }

        public void UpdateContacts(List<string> newContacts)
        {
            _contacts = newContacts.Select(name => new Contact(name, false)).ToList();
            OnContactListChanged?.Invoke();
        }

        public void UpdateContactsWithStatus(List<ContactInfo> contactsWithStatus)
        {
            _contacts = contactsWithStatus.Select(c => new Contact(c.Username, c.IsOnline)).ToList();
            OnContactListChanged?.Invoke();
        }

        public void AddContact(string name, bool status)
        {
            if (!_contacts.Any(c => c.Name == name))
            {
                _contacts.Add(new Contact(name, status));
                OnContactListChanged?.Invoke();
            }
        }

        public void AddContact(string contactUsername)
        {
            AddContact(contactUsername, false);
        }

        public void RemoveContact(string username)
        {
            var contact = _contacts.FirstOrDefault(c => c.Name == username);
            if (contact != null && _contacts.Remove(contact))
            {
                OnContactListChanged?.Invoke();
            }
        }

        public void UpdateContact(string username, bool isOnline)
        {
            var contact = _contacts.FirstOrDefault(c => c.Name == username);
            if (contact != null)
            {
                contact.IsOnline = isOnline;
                OnContactListChanged?.Invoke();
            }
        }

        public List<string> GetContacts()
        {
            return _contacts.Select(c => c.Name).ToList();
        }

        public List<ContactInfo> GetContactsWithStatus()
        {
            return _contacts.Select(c => new ContactInfo
            {
                Username = c.Name,
                IsOnline = c.IsOnline
            }).ToList();
        }

        public bool IsContact(string username)
        {
            return _contacts.Any(c => c.Name == username);
        }

        public bool WasContactRemoved(string currentContact, List<string> newContacts)
        {
            return !string.IsNullOrEmpty(currentContact) && !newContacts.Contains(currentContact);
        }

        public void AddRoom(Room room)
        {
            rooms.AddRoom(room);
        }

        public bool UserLeftRoom(string username, int roomId)
        {
            var room = rooms.GetRoom(roomId);
            if (room != null)
            {
                room.RemoveMember(username);
                return true;
            }
            return false;
        }

        public Room? GetRoom(int id)
        {
            return rooms.GetRoom(id);
        }
    }
}
