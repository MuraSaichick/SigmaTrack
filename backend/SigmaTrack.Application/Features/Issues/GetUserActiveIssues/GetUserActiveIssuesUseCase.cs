using FluentValidation;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SigmaTrack.Application.Features.Issues.GetUserActiveIssues;
public record UserActiveIssueResponse(
    Guid Id,
    Guid ProjectId,
    int Number,
    string Title,
    IssueStatus Status,
    IssueType Type,
    IssuePriority Priority,
    int? StoryPoints,
    Guid? AssigneeId,
    DateTime UpdatedAt
);
public interface IGetUserActiveIssuesUseCase
{
    Task<IReadOnlyCollection<UserActiveIssueResponse>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
public class GetUserActiveIssuesUseCase : IGetUserActiveIssuesUseCase
{
    private readonly IIssueRepository _issueRepository;
    public GetUserActiveIssuesUseCase(
        IIssueRepository issueRepository)
    {
        _issueRepository = issueRepository;
    }
    public async Task<IReadOnlyCollection<UserActiveIssueResponse>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var activeStatuses = new[]
        {
    IssueStatus.ToDo,
    IssueStatus.InProgress,
    IssueStatus.InReview,
    IssueStatus.Testing,
    IssueStatus.Reopened,
    IssueStatus.OnHold
};
        return await _issueRepository.GetActiveIssuesByAssigneeAsync(userId, activeStatuses, cancellationToken);
    }
}