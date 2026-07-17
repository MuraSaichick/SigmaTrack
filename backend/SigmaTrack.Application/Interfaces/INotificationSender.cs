namespace SigmaTrack.Application.Interfaces
{
    public interface INotificationSender
    {
        Task SendToUserAsync(Guid userId, string method, object payload, CancellationToken cancellationToken = default);
        Task SendToAllAsync(string method, object payload, CancellationToken cancellationToken = default);
    }
}
