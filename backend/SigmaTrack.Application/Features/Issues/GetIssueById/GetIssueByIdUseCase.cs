using FluentValidation;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SigmaTrack.Application.Features.Issues.GetIssueById;
public record GetIssueByIdQuery(Guid Id);
public record AttachmentResponseDto(
    Guid Id,
    string Filename,
    string FileUrl,
    long FileSize,
    string ContentType,
    DateTime UploadedAt
);
public record CommentDto(
    Guid Id,
    Guid AuthorId,
    string AuthorName,
    string Text,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<string> Mentions,
    List<AttachmentResponseDto> Attachments
    );

    public record IssueLinkDto(
    Guid Id,
    Guid TargetIssueId,
    string TargetIssueNumber,
    string TargetIssueTitle,
    IssueLinkType LinkType,
    string? Description
);
public record IssueDetailResponse(
    Guid Id,
    Guid ProjectId,
    int Number,
    string Title,
    string? Description,
    int Status,
    int Type,
    int Priority,
    int? Severity,
    Guid ReporterId,
    string ReporterName,
    Guid? AssigneeId,
    string? AssigneeName,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? DueDate,
    DateTime? StartedAt,
    DateTime? ResolvedAt,
    DateTime? ClosedAt,
    int? StoryPoints,
    double? EstimatedHours,
    double? LoggedHours,
    double? RemainingHours,
    string? Component,
    string? Version,
    string? FixVersion,
    string? Environment,
    List<string> Tags,
    bool IsReproducible,
    string? StepsToReproduce,
    bool IsBlocked,
    string? BlockReason,
    Guid? BlockedByIssueId,
    int ViewCount,
    List<CommentDto> Comments,
    List<IssueLinkDto> Links
);

public interface IGetIssueByIdUseCase
{
    Task<IssueDetailResponse> ExecuteAsync(GetIssueByIdQuery query, CancellationToken cancellationToken);
}
public class GetIssueByIdUseCase : IGetIssueByIdUseCase
{
    private readonly IIssueRepository _issueRepository;
    private readonly IValidator<GetIssueByIdQuery> _validator;

    public GetIssueByIdUseCase(IIssueRepository issueRepository, IValidator<GetIssueByIdQuery> validator)
    {
        _issueRepository = issueRepository;
        _validator = validator;
    }

    public async Task<IssueDetailResponse> ExecuteAsync(GetIssueByIdQuery query, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(query, cancellationToken);
        var issue = await _issueRepository.GetByIdWithDetailsAsync(query.Id, cancellationToken);

        if (issue == null)
            throw new ArgumentException($"Задача с ID {query.Id} не найдена.");
        var links = issue.SourceRelations.Select(l => new IssueLinkDto(
            l.Id,
            l.TargetIssueId,
            $"{issue.Project?.Name ?? "TASK"}-{l.TargetIssue?.Number}",
            l.TargetIssue?.Title ?? string.Empty,
            l.LinkType,
            l.Description
        )).ToList();
        var comments = issue.Comments.Select(c => new CommentDto(
            c.Id,
            c.AuthorId,
            c.Author?.Login ?? "Удаленный пользователь",
            c.Text,
            c.CreatedAt,
            c.UpdatedAt,
            c.Mentions,
            c.Attachments.Select(a => new AttachmentResponseDto(
                a.Id,
                a.Filename,
                a.FileUrl,
                a.FileSize,
                a.ContentType,
                a.UploadedAt
            )).ToList()
        )).OrderBy(c => c.CreatedAt).ToList();
        return new IssueDetailResponse(
            issue.Id,
            issue.ProjectId,
            issue.Number,
            issue.Title,
            issue.Description,
            (int)issue.Status,
            (int)issue.Type,
            (int)issue.Priority,
            issue.Severity.HasValue ? (int)issue.Severity.Value : null,
            issue.ReporterId,
            issue.Reporter?.Login ?? "Система",
            issue.AssigneeId,
            issue.Assignee?.Login,
            issue.CreatedAt,
            issue.UpdatedAt,
            issue.DueDate,
            issue.StartedAt,
            issue.ResolvedAt,
            issue.ClosedAt,
            issue.StoryPoints,
            issue.EstimatedHours,
            issue.LoggedHours,
            issue.RemainingHours,
            issue.Component,
            issue.Version,
            issue.FixVersion,
            issue.Environment,
            issue.Tags,
            issue.IsReproducible,
            issue.StepsToReproduce,
            issue.IsBlocked,
            issue.BlockReason,
            issue.BlockedByIssueId,
            issue.ViewCount,
            comments,
            links
        );
    }
}
public class GetIssueByIdQueryValidator : AbstractValidator<GetIssueByIdQuery>
{
    public GetIssueByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ID задачи обязателен.");
    }
}