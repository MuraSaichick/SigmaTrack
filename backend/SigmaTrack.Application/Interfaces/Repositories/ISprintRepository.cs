using SigmaTrack.Application.Features.Sprints.GetProjectSprints;
using SigmaTrack.Application.Features.Sprints.GetSprintDetails;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Interfaces.Repositories
{
    public interface ISprintRepository
    {
        Task AddAsync(Sprint sprint, CancellationToken cancellationToken = default);
        Task<Sprint?> GetByIdAsync(Guid sprintId, CancellationToken cancellationToken = default);
        Task<Sprint?> GetWithIssuesByIdAsync(Guid sprintId, CancellationToken cancellationToken = default);
        Task<Guid?> GetProjectIdAsync(Guid sprintId, CancellationToken cancellationToken = default);
        Task<IEnumerable<SprintLookupDto>> GetByProjectWithFiltersAsync(Guid projectId, SprintStatus? status, CancellationToken cancellationToken = default);
        Task<SprintDetailsResponse?> GetDetailsAsync(Guid sprintId, CancellationToken cancellationToken = default);
    }
}
