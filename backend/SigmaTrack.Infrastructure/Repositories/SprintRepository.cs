using Microsoft.EntityFrameworkCore;
using SigmaTrack.Application.Features.Sprints.GetProjectSprints;
using SigmaTrack.Application.Features.Sprints.GetSprintDetails;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;
using SigmaTrack.Infrastructure.Data;

namespace SigmaTrack.Infrastructure.Repositories
{
    public class SprintRepository : ISprintRepository
    {
        private readonly AppDbContext _context;

        public SprintRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Sprint sprint, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(sprint, cancellationToken);
        }
        public async Task<Sprint?> GetByIdAsync(Guid sprintId, CancellationToken cancellationToken = default)
        {
            return await _context.Sprints
                .FirstOrDefaultAsync(s => s.Id == sprintId, cancellationToken);
        }

        public async Task<Sprint?> GetWithIssuesByIdAsync(Guid sprintId, CancellationToken cancellationToken = default)
        {
            return await _context.Sprints
                .Include(s => s.Issues)
                .FirstOrDefaultAsync(s => s.Id == sprintId, cancellationToken);
        }
        public async Task<Guid?> GetProjectIdAsync(Guid sprintId, CancellationToken cancellationToken = default)
        {
            return await _context.Sprints
                .Where(s => s.Id == sprintId)
                .Select(s => (Guid?)s.ProjectId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<SprintLookupDto>> GetByProjectWithFiltersAsync(Guid projectId, SprintStatus? status, CancellationToken cancellationToken = default)
        {
            var query = _context.Sprints
                .AsNoTracking()
                .Where(s => s.ProjectId == projectId);
            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            return await query
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new SprintLookupDto(
                    s.Id,
                    s.Name,
                    s.Goal,
                    s.Status.ToString(),
                    s.StartDate,
                    s.EndDate,
                    s.Capacity,
                    s.CommittedPoints
                ))
                .ToListAsync(cancellationToken);
        }
        public async Task<SprintDetailsResponse?> GetDetailsAsync(Guid sprintId, CancellationToken cancellationToken = default)
        {
            return await _context.Sprints
                .AsNoTracking()
                .Where(s => s.Id == sprintId)
                .Select(s => new SprintDetailsResponse(
                    s.Id,
                    s.Name,
                    s.Goal,
                    s.Status.ToString(),
                    s.StartDate,
                    s.EndDate,
                    s.Capacity,
                    s.Status == SprintStatus.Completed || s.Status == SprintStatus.Cancelled
                        ? s.CommittedPoints
                        : s.Issues.Where(i => i.StoryPoints.HasValue).Sum(i => i.StoryPoints!.Value),
                    s.Status == SprintStatus.Completed || s.Status == SprintStatus.Cancelled
                        ? s.CompletedPoints
                        : s.Issues.Where(i => i.StoryPoints.HasValue &&
                                             (i.Status == IssueStatus.Closed || i.Status == IssueStatus.Resolved))
                                   .Sum(i => i.StoryPoints!.Value),

                    s.Issues.Select(i => new SprintIssueDto(
                        i.Id,
                        $"{i.Project.Prefix}-{i.Number}",
                        i.Title,
                        i.Status.ToString(),
                        i.Type.ToString(),
                        i.Priority.ToString(),
                        i.StoryPoints,
                        i.Assignee != null ? i.Assignee.Login : null
                    ))
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}