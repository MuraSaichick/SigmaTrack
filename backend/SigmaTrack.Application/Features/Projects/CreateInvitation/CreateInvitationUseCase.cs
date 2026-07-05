using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Enums;
using SigmaTrack.Domain.Entities;


namespace SigmaTrack.Application.Features.Projects.CreateInvitation
{


    public interface ICreateInvitationUseCase
    {
        Task<CreateInvitationResponse> ExecuteAsync(CreateInvitationCommand command, CancellationToken cancellationToken);
    }

    public class CreateInvitationUseCase : ICreateInvitationUseCase
    {
        private readonly IProjectInvitationRepository _invitationRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateInvitationUseCase(IProjectInvitationRepository invitationRepository, IProjectRepository projectRepository, IUserRepository userRepository, INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
        {
            _invitationRepository = invitationRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateInvitationResponse> ExecuteAsync(CreateInvitationCommand command, CancellationToken cancellationToken)
        {
            bool isManager = await _projectRepository.IsProjectMemberAsync(command.ProjectId, command.InviterId);
            if (!isManager)
            {
                throw new UnauthorizedAccessException("У вас нет прав для приглашения пользователей в этот проект.");
            }
            var invitee = await _userRepository.GetByEmailAsync(command.InviteeEmail);
            if (invitee == null)
            {
                throw new Exception($"Пользователь с email {command.InviteeEmail} не найден.");
            }
            bool isAlreadyMember = await _projectRepository.IsProjectMemberAsync(command.ProjectId, invitee.Id);
            if (isAlreadyMember)
            {
                throw new Exception("Этот пользователь уже является участником проекта.");
            }
            bool hasActiveInvite = await _invitationRepository.HasActiveInvitationAsync(command.ProjectId, invitee.Id);
            if (hasActiveInvite)
            {
                throw new Exception("Этому пользователю уже отправлено приглашение, ожидающее ответа.");
            }
            var invitation = new ProjectInvitation
            {
                Id = Guid.NewGuid(),
                ProjectId = command.ProjectId,
                InviterId = command.InviterId,
                InviteeId = invitee.Id,
                ProjectRoleId = command.ProjectRoleId,
                Status = InvitationStatus.Pending,
                InvitedAt = DateTime.UtcNow
            };
            await _invitationRepository.AddAsync(invitation);
            var role = (ProjectRoleEnum)command.ProjectRoleId;
            var notification = Notification.CreateProjectInvitationReceived(
                invitee.Id,
                role,
                invitation.Id
            );
            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new CreateInvitationResponse(invitation.Id, invitation.Status.ToString());
        }
    }
}
