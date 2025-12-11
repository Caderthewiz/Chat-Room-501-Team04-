using Shared;

namespace Client.Interfaces
{
    public interface IClientCommsController
    {
        event Action<CreateRoomResponse>? OnCreateRoomResponse;
        event Action<ChatMessage>? OnChatMessage;
        event Action<RoomListResponse>? OnRoomListResponse;
        event Action<JoinRoomResponse>? OnJoinRoomResponse;
        event Action<AddContactResponse>? OnAddContactResponse;
        event Action<ContactListResponse>? OnContactListResponse;
        event Action<string>? OnError;
        event Action? OnDisconnect;

        void ConnectToServer();
        void SendCreateRoom(string roomName, string password);
        void SendJoinRoom(string roomName, string password);
        void SendChatMessage(string roomName, string text);
        void SendAddContact(string contactUsername);
        void SendRemoveContact(string contactUsername);
        void Disconnect();
    }
}
