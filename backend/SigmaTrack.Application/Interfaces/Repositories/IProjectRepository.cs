using SigmaTrack.Application.Features.Projects.GetProjectMembers;
using SigmaTrack.Application.Features.Projects.GetUserProjects;
using SigmaTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Interfaces.Repositories
{
    public interface IProjectRepository
    {
        Task<Project?> GetByPrefixAsync(string prefix, CancellationToken cancellationToken);
        Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid projectId, CancellationToken cancellationToken = default);
        Task<bool> IsProjectManagerOrCreatorAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
        Task<bool> IsProjectMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
        Task<Project?> GetWithMembersByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProjectMemberDto>> GetMembersByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
        Task AddProjectMemberAsync(ProjectMember member, CancellationToken cancellationToken = default);
        Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<UserProjectDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task AddAsync(Project project, CancellationToken cancellationToken);
        Task DeleteAsync(Project project, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
