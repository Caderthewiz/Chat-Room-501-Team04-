using Shared;

namespace Server.Interfaces
{
    public interface IChatHistoryRepository
    {
        void AddMessage(string roomName, ChatMessage message);
        List<ChatMessage> GetHistory(string roomName);
        void ClearHistory(string roomName);
        bool HasHistory(string roomName);
        int RoomCount { get; }
    }
}
