using Shared;

namespace Server.Interfaces
{
    public interface IServerCommsController
    {
        void StartServer();
        void StopServer();
        void HandleLogin(LoginMessage loginMsg, IClientSession sender);
        void HandleCreateRoom(CreateRoomRequest request, IClientSession sender);
        void HandleJoinRoom(JoinRoomRequest request, IClientSession sender);
        void HandleChatMessage(ChatMessage chatMsg, IClientSession sender);
        void HandleAddContact(AddContactRequest request, IClientSession sender);
        void HandleRemoveContact(RemoveContactRequest request, IClientSession sender);
        void HandleDisconnect(string username);
        void SendRoomListToClient(IClientSession client);
        void SendContactListToClient(IClientSession client, string username);
        MessageDispatcher GetMessageDispatcher();
    }
}
