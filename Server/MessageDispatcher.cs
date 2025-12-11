using Newtonsoft.Json;
using Shared;
using Server.Interfaces;

namespace Server
{
    public class MessageDispatcher
    {
        private readonly Dictionary<string, Action<string, IClientSession>> handlers;

        public MessageDispatcher()
        {
            handlers = new Dictionary<string, Action<string, IClientSession>>();
        }

        public void RegisterHandler<T>(string messageType, Action<T, IClientSession> handler)
        {
            handlers[messageType] = (data, sender) =>
            {
                var message = JsonConvert.DeserializeObject<T>(data);
                if (message != null)
                {
                    handler(message, sender);
                }
            };
        }

        public void Dispatch(Shared.Message message, IClientSession sender)
        {
            if (handlers.TryGetValue(message.Type, out var handler))
            {
                handler(message.Data, sender);
            }
        }
    }
}
