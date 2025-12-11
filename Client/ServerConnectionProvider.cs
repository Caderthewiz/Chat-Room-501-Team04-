using Client.Interfaces;

namespace Client
{
    public class ServerConnectionProvider : IServerConnectionProvider
    {
        private readonly string serverUrl;

        public ServerConnectionProvider(string serverUrl = "ws://127.0.0.1:8080/login")
        {
            this.serverUrl = serverUrl;
        }

        public string GetServerUrl()
        {
            return serverUrl;
        }
    }
}
