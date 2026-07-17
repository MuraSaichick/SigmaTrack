using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;

namespace SigmaTrack.Application.Features.Notifications.MarkAsRead
{
    public interface IMarkAsReadUseCase
    {
        Task ExecuteAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken);
    }
    public class MarkAsReadUseCase : IMarkAsReadUseCase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkAsReadUseCase(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);

            if (notification == null) throw new Exception("Уведомление не найдено.");
            if (notification.UserId != userId) throw new UnauthorizedAccessException();
            if (!notification.IsRead)
            {
                notification.IsRead = true;
                _notificationRepository.Update(notification);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}