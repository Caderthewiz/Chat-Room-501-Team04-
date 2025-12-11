using Shared.Interfaces;

namespace Shared
{
    public class RoomTypeClassifier : IRoomTypeClassifier
    {
        private const string DM_PREFIX = "DM_";

        public bool IsDirectMessage(string roomName)
        {
            return !string.IsNullOrEmpty(roomName) && roomName.StartsWith(DM_PREFIX);
        }

        public bool IsPublicRoom(string roomName)
        {
            return !IsDirectMessage(roomName);
        }

        public string CreateDMRoomName(string user1, string user2)
        {
            // Sort alphabetically for consistency
            var users = new[] { user1, user2 }.OrderBy(u => u).ToArray();
            return $"{DM_PREFIX}{users[0]}_{users[1]}";
        }
    }
}
