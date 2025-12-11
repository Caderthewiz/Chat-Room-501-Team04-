using Shared;

namespace Server.Interfaces
{
    public interface IRoomManager
    {
        CreateRoomResponse CreateRoom(CreateRoomRequest request);
        JoinRoomResponse JoinRoom(JoinRoomRequest request, ref string userCurrentRoom);
        void LeaveRoom(string roomName, string username);
        RoomListResponse GetRoomList();
        List<Room> GetAllRooms();
    }
}
