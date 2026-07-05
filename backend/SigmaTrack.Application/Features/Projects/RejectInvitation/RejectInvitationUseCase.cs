using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;


namespace SigmaTrack.Application.Features.Projects.RejectInvitation
{
    public interface IRejectInvitationUseCase
    {
        Task ExecuteAsync(RejectInvitationCommand command, CancellationToken cancellationToken);
    }
    public class RejectInvitationUseCase : IRejectInvitationUseCase
    {
        private readonly IProjectInvitationRepository _invitationRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RejectInvitationUseCase(
        IProjectInvitationRepository invitationRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
        {
            _invitationRepository = invitationRepository;
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task ExecuteAsync(RejectInvitationCommand command, CancellationToken cancellationToken)
        {
            var invitation = await _invitationRepository.GetByIdAsync(command.InvitationId);
            if (invitation == null)
            {
                throw new Exception("Приглашение не найдено.");
            }
            if (invitation.InviteeId != command.UserId)
            {
                throw new UnauthorizedAccessException("Вы не можете отклонить это приглашение.");
            }
            if (invitation.Status != InvitationStatus.Pending)
            {
                throw new Exception("Это приглашение уже было обработано.");
            }
            invitation.Status = InvitationStatus.Rejected;
            invitation.RespondedAt = DateTime.UtcNow;
            await _invitationRepository.UpdateAsync(invitation);
            var inviteeName = invitation.Invitee?.Firstname ?? "Пользователь";
            var projectName = invitation.Project?.Name ?? "проект";

            var notification = Notification.CreateProjectInvitationRejected(
                invitation.InviterId,
                inviteeName,
                projectName
            );
            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}