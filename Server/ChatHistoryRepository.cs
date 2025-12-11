using Newtonsoft.Json;
using Shared;
using Server.Interfaces;

namespace Server
{
    public class ChatHistoryRepository : IChatHistoryRepository
    {
        private readonly Dictionary<string, List<ChatMessage>> chatHistory;
        private static readonly string ChatHistoryFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ChatApp",
            "chathistory.json"
        );

        public int RoomCount => chatHistory.Count;

        public ChatHistoryRepository()
        {
            chatHistory = new Dictionary<string, List<ChatMessage>>();
            LoadChatHistory();
        }

        public void AddMessage(string roomName, ChatMessage message)
        {
            if (!chatHistory.ContainsKey(roomName))
            {
                chatHistory[roomName] = new List<ChatMessage>();
            }
            chatHistory[roomName].Add(message);
            SaveChatHistory();
        }

        public List<ChatMessage> GetHistory(string roomName)
        {
            return chatHistory.ContainsKey(roomName) ? chatHistory[roomName] : new List<ChatMessage>();
        }

        public void ClearHistory(string roomName)
        {
            if (chatHistory.ContainsKey(roomName))
            {
                chatHistory.Remove(roomName);
                SaveChatHistory();
            }
        }

        public bool HasHistory(string roomName)
        {
            return chatHistory.ContainsKey(roomName);
        }

        private void LoadChatHistory()
        {
            try
            {
                if (File.Exists(ChatHistoryFilePath))
                {
                    using (var fileStream = new FileStream(ChatHistoryFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(fileStream))
                    {
                        string json = reader.ReadToEnd();
                        var history = JsonConvert.DeserializeObject<Dictionary<string, List<ChatMessage>>>(json);

                        if (history != null)
                        {
                            foreach (var kvp in history)
                            {
                                chatHistory[kvp.Key] = kvp.Value;
                            }
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(ChatHistoryFilePath)!);
                }
            }
            catch (Exception)
            {
                // If load fails, start with empty history
            }
        }

        private void SaveChatHistory()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ChatHistoryFilePath)!);
                string json = JsonConvert.SerializeObject(chatHistory, Formatting.Indented);

                using (var fileStream = new FileStream(ChatHistoryFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(fileStream))
                {
                    writer.Write(json);
                    writer.Flush();
                }
            }
            catch (Exception)
            {
                // Silently fail on save errors
            }
        }
    }
}
