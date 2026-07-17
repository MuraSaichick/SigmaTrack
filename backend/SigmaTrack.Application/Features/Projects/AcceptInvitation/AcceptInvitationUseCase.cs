using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.AcceptInvitation
{
    public interface IAcceptInvitationUseCase
    {
        Task ExecuteAsync(AcceptInvitationCommand command, CancellationToken cancellationToken);
    }
    public class AcceptInvitationUseCase : IAcceptInvitationUseCase
    {
        private readonly IProjectInvitationRepository _invitationRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationSender _notificationSender;
        public AcceptInvitationUseCase(
        IProjectInvitationRepository invitationRepository,
        IProjectRepository projectRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        INotificationSender notificationSender)
        {
            _invitationRepository = invitationRepository;
            _projectRepository = projectRepository;
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _notificationSender = notificationSender;
        }
        public async Task ExecuteAsync(AcceptInvitationCommand command, CancellationToken cancellationToken)
        {
            var invitation = await _invitationRepository.GetByIdAsync(command.InvitationId);
            if (invitation == null)
            {
                throw new Exception("Приглашение не найдено.");
            }

            if (invitation.InviteeId != command.UserId)
            {
                throw new UnauthorizedAccessException("Вы не можете принять это приглашение.");
            }

            if (invitation.Status != InvitationStatus.Pending)
            {
                throw new Exception($"Нельзя принять приглашение со статусом {invitation.Status}.");
            }
            invitation.Status = InvitationStatus.Accepted;
            invitation.RespondedAt = DateTime.UtcNow;
            await _invitationRepository.UpdateAsync(invitation);
            var newMember = new ProjectMember(invitation.ProjectId, invitation.InviteeId, invitation.ProjectRoleId);
            await _projectRepository.AddProjectMemberAsync(newMember);
            var inviteeName = invitation.Invitee?.Firstname ?? "Пользователь";
            var projectName = invitation.Project?.Name ?? "проект";

            var notification = Notification.CreateProjectInvitationAccepted(
                invitation.InviterId,
                inviteeName,
                projectName
            );
            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _notificationSender.SendToUserAsync(
               notification.UserId,
               "ReceiveNotification",
               notification,
               cancellationToken
           );
        }
    }
}
