namespace Shared
{
    public class Room
    {
        public static int nextID = 0;

        public int ID { get; }
        public string Name { get; set; }
        public string Password { get; set; }
        public List<string> Clients { get; set; }
        public List<string> Messages { get; set; }

        public Room(string name, string password)
        {
            ID = nextID++;
            Name = name;
            Password = password;
            Clients = new List<string>();
            Messages = new List<string>();
        }

        // Constructor for tests - creates a room with clients
        public Room(List<string> clients)
        {
            ID = nextID++;
            Name = $"Room_{ID}";
            Password = "";
            Clients = new List<string>(clients);
            Messages = new List<string>();
        }

        public void AddMember(string username)
        {
            if (!Clients.Contains(username))
            {
                Clients.Add(username);
            }
        }

        public void RemoveMember(string username)
        {
            Clients.Remove(username);
        }

        public void AddMessage(string message)
        {
            Messages.Add(message);
        }

        public void AddClient(string username)
        {
            if (!Clients.Contains(username))
            {
                Clients.Add(username);
            }
        }

        public void RemoveClient(string username)
        {
            Clients.Remove(username);
        }

        public string GetRoomName()
        {
            return string.Join(", ", Clients);
        }

        public override string ToString()
        {
            return $"Room {ID}: {string.Join(", ", Clients)} ({Clients.Count} clients)";
        }
    }
}
