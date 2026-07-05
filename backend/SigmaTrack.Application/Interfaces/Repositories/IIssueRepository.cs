using SigmaTrack.Application.Features.Issues.GetIssuesList;
using SigmaTrack.Application.Features.Issues.GetUserActiveIssues;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Interfaces.Repositories
{
    public interface IIssueRepository
    {
        Task AddAsync(Issue issue, CancellationToken cancellationToken);
        Task<int> GetNextNumberAsync(Guid projectId, CancellationToken cancellationToken);
        Task<(IReadOnlyCollection<IssueDto> Items, int TotalCount)> GetListAsync(GetIssuesListQuery query, CancellationToken cancellationToken);
        Task<Issue?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Issue?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
        Task IncrementCommentCountAsync(Guid issueId, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid issueId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<UserActiveIssueResponse>> GetActiveIssuesByAssigneeAsync(
        Guid userId,
        IEnumerable<IssueStatus> statuses,
        CancellationToken cancellationToken);

    }
}
