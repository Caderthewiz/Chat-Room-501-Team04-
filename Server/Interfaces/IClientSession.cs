namespace Server.Interfaces
{
    public interface IClientSession
    {
        string? Username { get; set; }
        string? CurrentRoom { get; set; }
        void SendMessage(string type, object data);
    }
}
