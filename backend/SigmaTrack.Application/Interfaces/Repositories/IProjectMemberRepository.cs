using SigmaTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Interfaces.Repositories
{
    public interface IProjectMemberRepository
    {
        Task<ProjectMember?> GetByIdAsync(Guid userId, Guid projectId, CancellationToken cancellationToken);
        Task<IEnumerable<ProjectMember>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken);
        Task<bool> IsMemberAsync(Guid userId, Guid projectId, CancellationToken cancellationToken);
        Task AddAsync(ProjectMember member, CancellationToken cancellationToken);
        Task UpdateAsync(ProjectMember member, CancellationToken cancellationToken);
        Task RemoveAsync(ProjectMember member, CancellationToken cancellationToken);
        Task<int?> GetRoleIdAsync(Guid userId, Guid projectId, CancellationToken cancellationToken);
    }
}
