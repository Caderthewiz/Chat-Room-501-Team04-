using Newtonsoft.Json;
using Shared;
using Client.Interfaces;

namespace Client
{
    public class ClientMessageDispatcher
    {
        private readonly Dictionary<string, Action<string>> handlers;

        public ClientMessageDispatcher()
        {
            handlers = new Dictionary<string, Action<string>>();
        }

        public void RegisterHandler<T>(string messageType, Action<T> handler)
        {
            handlers[messageType] = (data) =>
            {
                var message = JsonConvert.DeserializeObject<T>(data);
                if (message != null)
                {
                    handler(message);
                }
            };
        }

        public void Dispatch(Shared.Message message)
        {
            if (handlers.TryGetValue(message.Type, out var handler))
            {
                handler(message.Data);
            }
        }
    }
}
