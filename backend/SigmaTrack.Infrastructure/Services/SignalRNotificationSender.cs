using Microsoft.AspNetCore.SignalR;
using SigmaTrack.Application.Interfaces;
namespace SigmaTrack.Infrastructure.Services
{
    public class SignalRNotificationSender : INotificationSender
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationSender(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(Guid userId, string method, object payload, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync(method, payload, cancellationToken);
        }
        public async Task SendToAllAsync(string method, object payload, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.All.SendAsync(method, payload, cancellationToken);
        }
    }
}
