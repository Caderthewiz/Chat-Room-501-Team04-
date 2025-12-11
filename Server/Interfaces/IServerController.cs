namespace Server.Interfaces
{
    // Composite interface that extends all focused interfaces for backward compatibility
    public interface IServerController : IRoomManager, IChatHistoryManager, IContactManager, IUserManager, IServerEvents
    {
        // No additional members needed - all inherited from focused interfaces
    }
}
