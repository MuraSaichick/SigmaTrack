using Microsoft.EntityFrameworkCore;
using SigmaTrack.Application.Features.Issues.GetIssuesList;
using SigmaTrack.Application.Features.Issues.GetUserActiveIssues;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;
using SigmaTrack.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SigmaTrack.Infrastructure.Repositories;
public class IssueRepository : IIssueRepository
{
    private readonly AppDbContext _context;
    public IssueRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Issue issue, CancellationToken cancellationToken)
    {
        await _context.Issues.AddAsync(issue, cancellationToken);
    }
    public async Task<int> GetNextNumberAsync(Guid projectId, CancellationToken cancellationToken)
    {
        int? maxNumber = await _context.Issues
            .Where(i => i.ProjectId == projectId)
            .Select(i => (int?)i.Number)
            .MaxAsync(cancellationToken);
        return (maxNumber ?? 0) + 1;
    }
    public async Task<(IReadOnlyCollection<IssueDto> Items, int TotalCount)> GetListAsync(GetIssuesListQuery query, CancellationToken cancellationToken)
    {
        var dbQuery = _context.Issues.AsNoTracking()
            .Where(i => i.ProjectId == query.ProjectId);
        if (query.Status.HasValue)
            dbQuery = dbQuery.Where(i => i.Status == query.Status.Value);

        if (query.Type.HasValue)
            dbQuery = dbQuery.Where(i => i.Type == query.Type.Value);

        if (query.Priority.HasValue)
            dbQuery = dbQuery.Where(i => i.Priority == query.Priority.Value);

        if (query.AssigneeId.HasValue)
            dbQuery = dbQuery.Where(i => i.AssigneeId == query.AssigneeId.Value);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var search = query.SearchTerm.Trim().ToLower();
            if (int.TryParse(search, out var num))
                dbQuery = dbQuery.Where(i => i.Title.ToLower().Contains(search) || i.Number == num);
            else
                dbQuery = dbQuery.Where(i => i.Title.ToLower().Contains(search));
        }
        int totalCount = await dbQuery.CountAsync(cancellationToken);
        var items = await dbQuery
            .OrderByDescending(i => i.UpdatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(i => new IssueDto(
                i.Id,
                i.Number,
                i.Title,
                (int)i.Status,
                (int)i.Type,
                (int)i.Priority,
                i.StoryPoints,
                i.AssigneeId,
                i.UpdatedAt
            ))
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
    public async Task<Issue?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Issues
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }
    public async Task<Issue?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Issues
            .Include(i => i.Project)
            .Include(i => i.Reporter)
            .Include(i => i.Assignee)
            .Include(i => i.Comments)
                .ThenInclude(c => c.Author)
            .Include(i => i.Comments)
                .ThenInclude(c => c.Attachments)
            .Include(i => i.SourceRelations)
                .ThenInclude(l => l.TargetIssue)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }
    public async Task IncrementCommentCountAsync(Guid issueId, CancellationToken cancellationToken)
    {
        await _context.Issues
            .Where(i => i.Id == issueId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(i => i.CommentCount, i => i.CommentCount + 1)
                .SetProperty(i => i.UpdatedAt, DateTime.UtcNow),
            cancellationToken);
    }
    public async Task<bool> ExistsAsync(Guid issueId, CancellationToken cancellationToken)
    {
        return await _context.Issues.AnyAsync(i => i.Id == issueId, cancellationToken);
    }
    public async Task<IReadOnlyCollection<UserActiveIssueResponse>> GetActiveIssuesByAssigneeAsync(
        Guid userId,
        IEnumerable<IssueStatus> statuses,
        CancellationToken cancellationToken)
    {
        return await _context.Issues
            .AsNoTracking()
            .Where(i => i.AssigneeId == userId && statuses.Contains(i.Status))
            .OrderByDescending(i => i.UpdatedAt)
            .Select(i => new UserActiveIssueResponse(
                i.Id,
                i.ProjectId,
                i.Number,
                i.Title,
                i.Status,
                i.Type,
                i.Priority,
                i.StoryPoints,
                i.AssigneeId,
                i.UpdatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}