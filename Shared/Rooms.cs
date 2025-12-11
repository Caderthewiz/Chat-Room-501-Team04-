namespace Shared
{
    public class Rooms
    {
        private readonly List<Room> _rooms;

        public int Count => _rooms.Count;

        public Rooms()
        {
            _rooms = new List<Room>();
        }

        public Room? AddRoom(Room room)
        {
            if (!_rooms.Any(r => r.ID == room.ID && r.Name == room.Name))
            {
                _rooms.Add(room);
                return room;
            }
            return null;
        }

        public void RemoveRoom(string roomName)
        {
            var room = _rooms.FirstOrDefault(r => r.Name == roomName);
            if (room != null)
            {
                _rooms.Remove(room);
            }
        }

        public Room? GetRoom(string roomName)
        {
            return _rooms.FirstOrDefault(r => r.Name == roomName);
        }

        public Room? GetRoom(int ID)
        {
            return _rooms.FirstOrDefault(r => r.ID == ID);
        }

        public List<Room> GetRooms()
        {
            return _rooms.ToList();
        }

        public Room CreateRoom(List<string> clients)
        {
            var room = new Room($"Room_{Room.nextID}", "");
            foreach (var client in clients)
            {
                room.Clients.Add(client);
            }
            _rooms.Add(room);
            return room;
        }

        public List<Room> GetPublicRooms()
        {
            return _rooms.Where(r => !r.Name.StartsWith("DM_")).ToList();
        }

        public bool Contains(string roomName)
        {
            return _rooms.Any(r => r.Name == roomName);
        }

        public void Clear()
        {
            _rooms.Clear();
        }

        public void DeleteRoom(int id)
        {
            var room = _rooms.FirstOrDefault(r => r.ID == id);
            if (room != null)
            {
                _rooms.Remove(room);
            }
        }

        public List<Room> GetRoomsWithClient(string clientName)
        {
            return _rooms.Where(r => r.Clients.Contains(clientName)).ToList();
        }
    }
}
