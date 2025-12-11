using Newtonsoft.Json;
using Shared.Interfaces;

namespace Shared
{
    public class CredentialsM : ICredentialRepository
    {
        private static readonly string CredentialsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ChatApp",
            "credentials.json"
        );

        private Dictionary<string, UserCredential> _credentials;

        public CredentialsM()
        {
            _credentials = new Dictionary<string, UserCredential>();
            LoadCredentials();
        }

        public bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            if (_credentials.TryGetValue(username, out var credential))
            {
                return credential.Password == password;
            }

            return false;
        }

        public bool CreateUser(string username, string password, bool isAdmin)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            if (_credentials.ContainsKey(username))
                return false;

            _credentials[username] = new UserCredential
            {
                Username = username,
                Password = password,
                IsAdmin = isAdmin
            };

            SaveCredentials();
            return true;
        }

        public bool UserExists(string username)
        {
            return _credentials.ContainsKey(username);
        }

        public bool IsAdmin(string username)
        {
            if (_credentials.TryGetValue(username, out var credential))
            {
                return credential.IsAdmin;
            }
            return false;
        }

        public bool IsUserAdmin(string username)
        {
            return IsAdmin(username);
        }

        public List<string> GetAllUsernames()
        {
            return _credentials.Keys.ToList();
        }

        public void ReloadCredentials()
        {
            LoadCredentials();
        }

        private void LoadCredentials()
        {
            int retryCount = 0;
            int maxRetries = 5;

            while (retryCount < maxRetries)
            {
                try
                {
                    if (File.Exists(CredentialsFilePath))
                    {
                        using (var fileStream = new FileStream(CredentialsFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var reader = new StreamReader(fileStream))
                        {
                            string json = reader.ReadToEnd();
                            var credentials = JsonConvert.DeserializeObject<List<UserCredential>>(json);

                            if (credentials != null)
                            {
                                _credentials = credentials.ToDictionary(c => c.Username, c => c);
                            }
                        }
                    }
                    else
                    {
                        // Create directory if it doesn't exist
                        Directory.CreateDirectory(Path.GetDirectoryName(CredentialsFilePath)!);
                    }
                    return; // Success
                }
                catch (IOException)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        _credentials = new Dictionary<string, UserCredential>();
                        return;
                    }
                    Thread.Sleep(100); // Wait before retry
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading credentials: {ex.Message}");
                    _credentials = new Dictionary<string, UserCredential>();
                    return;
                }
            }
        }

        private void SaveCredentials()
        {
            int retryCount = 0;
            int maxRetries = 10;

            while (retryCount < maxRetries)
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(CredentialsFilePath)!);

                    // Reload credentials to merge any changes from other instances
                    Dictionary<string, UserCredential> currentCredentials = new Dictionary<string, UserCredential>();
                    if (File.Exists(CredentialsFilePath))
                    {
                        using (var fileStream = new FileStream(CredentialsFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var reader = new StreamReader(fileStream))
                        {
                            string existingJson = reader.ReadToEnd();
                            var existingCredentials = JsonConvert.DeserializeObject<List<UserCredential>>(existingJson);
                            if (existingCredentials != null)
                            {
                                currentCredentials = existingCredentials.ToDictionary(c => c.Username, c => c);
                            }
                        }
                    }

                    // Merge: keep existing users, add new ones from _credentials
                    foreach (var kvp in _credentials)
                    {
                        currentCredentials[kvp.Key] = kvp.Value;
                    }

                    var credentialList = currentCredentials.Values.ToList();
                    string json = JsonConvert.SerializeObject(credentialList, Formatting.Indented);

                    using (var fileStream = new FileStream(CredentialsFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (var writer = new StreamWriter(fileStream))
                    {
                        writer.Write(json);
                    }

                    return; // Success
                }
                catch (IOException)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        Console.WriteLine("Error saving credentials: exceeded retry count");
                        return;
                    }
                    Thread.Sleep(100); // Wait before retry
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving credentials: {ex.Message}");
                    return;
                }
            }
        }

        private class UserCredential
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public bool IsAdmin { get; set; }
        }
    }
}
