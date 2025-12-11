namespace Shared.Interfaces
{
    public interface IRoomTypeClassifier
    {
        bool IsDirectMessage(string roomName);
        bool IsPublicRoom(string roomName);
        string CreateDMRoomName(string user1, string user2);
    }
}
