using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Shared;
using Server.Interfaces;

namespace Server
{
    public class ChatServerBehavior : WebSocketBehavior, IClientSession
    {
        public IServerCommsController? ServerCommsController { get; set; }
        public string? Username { get; set; }
        public string? CurrentRoom { get; set; }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (ServerCommsController == null) return;

            var msg = JsonConvert.DeserializeObject<Shared.Message>(e.Data);
            if (msg == null) return;

            var dispatcher = ServerCommsController.GetMessageDispatcher();
            dispatcher.Dispatch(msg, this);
        }

        protected override void OnOpen()
        {
            if (ServerCommsController != null)
            {
                ServerCommsController.SendRoomListToClient(this);
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            if (ServerCommsController != null && !string.IsNullOrEmpty(Username))
            {
                ServerCommsController.HandleDisconnect(Username);
            }
        }

        public void SendMessage(string type, object data)
        {
            var msg = new Shared.Message(type, JsonConvert.SerializeObject(data));
            Send(JsonConvert.SerializeObject(msg));
        }
    }
}
