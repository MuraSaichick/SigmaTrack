using SigmaTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Interfaces.Repositories
{
    public interface IProjectInvitationRepository
    {
        Task<ProjectInvitation?> GetByIdAsync(Guid id);
        Task<IEnumerable<ProjectInvitation>> GetPendingInvitationsByUserIdAsync(Guid userId);
        Task AddAsync(ProjectInvitation invitation);
        Task UpdateAsync(ProjectInvitation invitation);
        Task<bool> HasActiveInvitationAsync(Guid projectId, Guid inviteeId);
    }
}
