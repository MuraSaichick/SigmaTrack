using SigmaTrack.Application.Interfaces.Repositories;

namespace SigmaTrack.Application.Features.Projects.GetProjectMembers
{
    public interface IGetProjectMembersUseCase
    {
        Task<IEnumerable<ProjectMemberDto>> ExecuteAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
    }

    public class GetProjectMembersUseCase : IGetProjectMembersUseCase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectAccessValidator _accessValidator;
        public GetProjectMembersUseCase(IProjectRepository projectRepository, IProjectAccessValidator projectAccessValidator)
        {
            _projectRepository = projectRepository;
            _accessValidator = projectAccessValidator;
        }

        public async Task<IEnumerable<ProjectMemberDto>> ExecuteAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
        {
            await _accessValidator.ValidateMemberAsync(projectId, userId, cancellationToken);
            if (projectId == Guid.Empty)
                throw new ArgumentException("Идентификатор проекта не может быть пустым.", nameof(projectId));
            return await _projectRepository.GetMembersByProjectIdAsync(projectId);
        }
    }
}
