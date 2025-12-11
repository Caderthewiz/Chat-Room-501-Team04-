using Newtonsoft.Json;
using Shared.Interfaces;

namespace Shared
{
    public class ContactsManager : IContactRepository
    {
        private static readonly string ContactsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ChatApp",
            "contacts.json"
        );

        private Dictionary<string, List<string>> _userContacts;

        public ContactsManager()
        {
            _userContacts = new Dictionary<string, List<string>>();
            LoadContacts();
        }

        public bool AddContact(string username, string contactUsername)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(contactUsername))
                return false;

            if (username == contactUsername)
                return false; // Can't add yourself

            // Ensure user has a contact list
            if (!_userContacts.ContainsKey(username))
            {
                _userContacts[username] = new List<string>();
            }

            // Check if already a contact
            if (_userContacts[username].Contains(contactUsername))
                return false;

            // Add contact
            _userContacts[username].Add(contactUsername);
            SaveContacts();
            return true;
        }

        public bool RemoveContact(string username, string contactUsername)
        {
            if (!_userContacts.ContainsKey(username))
                return false;

            bool removed = _userContacts[username].Remove(contactUsername);
            if (removed)
            {
                SaveContacts();
            }
            return removed;
        }

        public List<string> GetContacts(string username)
        {
            if (_userContacts.ContainsKey(username))
            {
                return new List<string>(_userContacts[username]);
            }
            return new List<string>();
        }

        public bool AreContacts(string username1, string username2)
        {
            if (_userContacts.ContainsKey(username1))
            {
                return _userContacts[username1].Contains(username2);
            }
            return false;
        }

        public List<string> GetUsersWhoHaveContact(string username)
        {
            var users = new List<string>();
            foreach (var kvp in _userContacts)
            {
                if (kvp.Value.Contains(username))
                {
                    users.Add(kvp.Key);
                }
            }
            return users;
        }

        private void LoadContacts()
        {
            int retryCount = 0;
            int maxRetries = 5;

            while (retryCount < maxRetries)
            {
                try
                {
                    if (File.Exists(ContactsFilePath))
                    {
                        using (var fileStream = new FileStream(ContactsFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var reader = new StreamReader(fileStream))
                        {
                            string json = reader.ReadToEnd();
                            var contacts = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

                            if (contacts != null)
                            {
                                _userContacts = contacts;
                            }
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(ContactsFilePath)!);
                    }
                    return;
                }
                catch (IOException)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        _userContacts = new Dictionary<string, List<string>>();
                        return;
                    }
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading contacts: {ex.Message}");
                    _userContacts = new Dictionary<string, List<string>>();
                    return;
                }
            }
        }

        private void SaveContacts()
        {
            int retryCount = 0;
            int maxRetries = 10;

            while (retryCount < maxRetries)
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(ContactsFilePath)!);

                    // Reload to merge changes
                    Dictionary<string, List<string>> currentContacts = new Dictionary<string, List<string>>();
                    if (File.Exists(ContactsFilePath))
                    {
                        using (var fileStream = new FileStream(ContactsFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var reader = new StreamReader(fileStream))
                        {
                            string existingJson = reader.ReadToEnd();
                            var existing = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(existingJson);
                            if (existing != null)
                            {
                                currentContacts = existing;
                            }
                        }
                    }

                    // Merge: update with _userContacts
                    foreach (var kvp in _userContacts)
                    {
                        currentContacts[kvp.Key] = kvp.Value;
                    }

                    string json = JsonConvert.SerializeObject(currentContacts, Formatting.Indented);

                    using (var fileStream = new FileStream(ContactsFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (var writer = new StreamWriter(fileStream))
                    {
                        writer.Write(json);
                    }

                    return;
                }
                catch (IOException)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        Console.WriteLine("Error saving contacts: exceeded retry count");
                        return;
                    }
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving contacts: {ex.Message}");
                    return;
                }
            }
        }
    }
}
