using SigmaTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.GetUserProjects
{
    public interface IGetUserProjectsUseCase
    {
        Task<IEnumerable<UserProjectDto>> ExecuteAsync(Guid userId, CancellationToken cancellationToken);
    }

    public class GetUserProjectsUseCase : IGetUserProjectsUseCase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;


        public GetUserProjectsUseCase(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<UserProjectDto>> ExecuteAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _projectRepository.GetByUserIdAsync(userId, cancellationToken);
        }
    }
}