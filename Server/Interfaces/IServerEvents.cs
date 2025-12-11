namespace Server.Interfaces
{
    public interface IServerEvents
    {
        event Action<string>? OnLogUpdate;
        event Action? OnRoomListChanged;
        event Action? OnUserListChanged;
        event Action<string>? OnUserStatusChanged;
    }
}
