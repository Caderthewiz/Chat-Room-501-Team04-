using Shared;
using Xunit;

namespace Tests
{
    public class RoomAndChatTests
    {
        public RoomAndChatTests()
        {
            // Reset Room.nextID for consistent test results
            Room.nextID = 0;
        }

        [Fact]
        public void TestCreateRoom()
        {
            var rooms = new Rooms();
            var clients = new List<string> { "Alice", "Bob" };
            var room = rooms.CreateRoom(clients);

            Assert.NotNull(room);
            Assert.Equal(2, room.Clients.Count);
            Assert.Contains("Alice", room.Clients);
            Assert.Contains("Bob", room.Clients);
            Assert.NotNull(room.Messages);
        }

        [Fact]
        public void TestAddRoom()
        {
            var rooms = new Rooms();
            var clients = new List<string> { "Alice" };
            var room = new Room(clients); // Create room directly without adding to collection
            var addedRoom = rooms.AddRoom(room);

            Assert.NotNull(addedRoom);
            Assert.Equal(room.ID, addedRoom.ID);
            Assert.Single(rooms.GetRooms());
        }

        [Fact]
        public void TestGetRoomById()
        {
            var rooms = new Rooms();
            var clients = new List<string> { "Alice" };
            var room = rooms.CreateRoom(clients);
            rooms.AddRoom(room);

            var retrievedRoom = rooms.GetRoom(room.ID);

            Assert.NotNull(retrievedRoom);
            Assert.Equal(room.ID, retrievedRoom.ID);
            Assert.Contains("Alice", retrievedRoom.Clients);
        }

        [Fact]
        public void TestAddMessageToRoom()
        {
            var rooms = new Rooms();
            var clients = new List<string> { "Alice" };
            var room = rooms.CreateRoom(clients);
            rooms.AddRoom(room);

            room.AddMessage("Hello, World!");
            room.AddMessage("Second message");

            Assert.Equal(2, room.Messages.Count);
            Assert.Equal("Hello, World!", room.Messages[0]);
            Assert.Equal("Second message", room.Messages[1]);
        }

        [Fact]
        public void TestGetRoomsWithClient()
        {
            var rooms = new Rooms();

            var room1 = rooms.CreateRoom(new List<string> { "Alice", "Bob" });
            var room2 = rooms.CreateRoom(new List<string> { "Alice", "Charlie" });
            var room3 = rooms.CreateRoom(new List<string> { "Bob", "Charlie" });

            rooms.AddRoom(room1);
            rooms.AddRoom(room2);
            rooms.AddRoom(room3);

            var aliceRooms = rooms.GetRoomsWithClient("Alice");
            var bobRooms = rooms.GetRoomsWithClient("Bob");
            var davidRooms = rooms.GetRoomsWithClient("David");

            Assert.Equal(2, aliceRooms.Count);
            Assert.Equal(2, bobRooms.Count);
            Assert.Empty(davidRooms);
        }

        [Fact]
        public void TestRoomNameGeneration()
        {
            var clients = new List<string> { "Alice", "Bob", "Charlie" };
            var room = new Room(clients);

            string roomName = room.GetRoomName();

            Assert.Equal("Alice, Bob, Charlie", roomName);
        }

        [Fact]
        public void TestAddRemoveClientFromRoom()
        {
            var clients = new List<string> { "Alice", "Bob" };
            var room = new Room(clients);

            room.AddClient("Charlie");
            Assert.Equal(3, room.Clients.Count);
            Assert.Contains("Charlie", room.Clients);

            room.RemoveClient("Bob");
            Assert.Equal(2, room.Clients.Count);
            Assert.DoesNotContain("Bob", room.Clients);
        }

        [Fact]
        public void TestDeleteRoom()
        {
            var rooms = new Rooms();
            var room1 = rooms.CreateRoom(new List<string> { "Alice" });
            var room2 = rooms.CreateRoom(new List<string> { "Bob" });

            rooms.AddRoom(room1);
            rooms.AddRoom(room2);

            int room1Id = room1.ID;
            rooms.DeleteRoom(room1Id);

            Assert.Single(rooms.GetRooms());
            Assert.Null(rooms.GetRoom(room1Id));
            Assert.NotNull(rooms.GetRoom(room2.ID));
        }

        [Fact]
        public void TestRoomToString()
        {
            var clients = new List<string> { "Alice", "Bob" };
            var room = new Room(clients);

            string result = room.ToString();

            Assert.Contains("Room", result);
            Assert.Contains(room.ID.ToString(), result);
            Assert.Contains("Alice", result);
            Assert.Contains("Bob", result);
        }

        [Fact]
        public void TestChatMessageCreation()
        {
            var chatMsg = new ChatMessage
            {
                RoomName = "TestRoom",
                Username = "Alice",
                Text = "Hello, Bob!",
                Timestamp = DateTime.Now
            };

            Assert.Equal("Alice", chatMsg.Username);
            Assert.Equal("TestRoom", chatMsg.RoomName);
            Assert.Equal("Hello, Bob!", chatMsg.Text);
            Assert.NotEqual(default(DateTime), chatMsg.Timestamp);
        }
    }
}
