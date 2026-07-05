using SigmaTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Notifications.GetNotifications
{
    public interface IGetNotificationsUseCase
    {
        Task<IEnumerable<NotificationDto>> ExecuteAsync(Guid userId, bool? isRead, CancellationToken cancellationToken);
    }
    public class GetNotificationsUseCase : IGetNotificationsUseCase
    {
        private readonly INotificationRepository _notificationRepository;

        public GetNotificationsUseCase(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }
        public async Task<IEnumerable<NotificationDto>> ExecuteAsync(Guid userId, bool? isRead, CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId, isRead, cancellationToken);

            return notifications.Select(n => new NotificationDto(
                n.Id,
                n.Type,
                n.Title,
                n.Message,
                n.IsRead,
                n.LinkUrl,
                n.CreatedAt
            )).ToList();
        }
    }
}
