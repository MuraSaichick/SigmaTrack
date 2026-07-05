using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;

namespace SigmaTrack.Application.Features.Projects.LeaveProject
{
    public record LeaveProjectCommand(Guid ProjectId, Guid UserId);

    public interface ILeaveProjectUseCase
    {
        Task ExecuteAsync(LeaveProjectCommand command, CancellationToken cancellationToken);
    }
    public class LeaveProjectUseCase : ILeaveProjectUseCase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        public LeaveProjectUseCase(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task ExecuteAsync(LeaveProjectCommand command, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetWithMembersByIdAsync(command.ProjectId, cancellationToken);
            if (project == null)
            {
                throw new KeyNotFoundException("Проект не найден.");
            }
            project.LeaveProject(command.UserId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
