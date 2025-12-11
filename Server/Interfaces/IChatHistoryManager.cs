using Shared;

namespace Server.Interfaces
{
    public interface IChatHistoryManager
    {
        void HandleChatMessage(ChatMessage chatMsg);
        List<ChatMessage> GetChatHistory(string roomName);
    }
}
