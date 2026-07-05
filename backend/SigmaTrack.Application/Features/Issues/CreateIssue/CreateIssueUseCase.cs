using FluentValidation;
using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;

namespace SigmaTrack.Application.Features.Issues.CreateIssue;
public record CreateIssueRequest(
    Guid ProjectId,
    string Title,
    string? Description,
    IssueType Type,
    IssuePriority Priority,
    Guid? AssigneeId,
    int? StoryPoints,
    List<string>? Tags
);

public record CreateIssueCommand(
    Guid ProjectId,
    Guid ReporterId,
    string Title,
    string? Description,
    IssueType Type,
    IssuePriority Priority,
    Guid? AssigneeId,
    int? StoryPoints,
    List<string> Tags
);
public record CreateIssueResponse(
    Guid Id,
    int Number,
    string Title,
    IssueStatus Status,
    Guid ReporterId,
    Guid? AssigneeId,
    List<string> Tags
);
public interface ICreateIssueUseCase
{
    Task<CreateIssueResponse> ExecuteAsync(CreateIssueCommand command, CancellationToken cancellationToken);
}
public class CreateIssueUseCase : ICreateIssueUseCase
{
    private readonly IIssueRepository _issueRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateIssueCommand> _validator;

    public CreateIssueUseCase(
        IIssueRepository issueRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateIssueCommand> validator,
        INotificationRepository notificationRepository)
    {
        _issueRepository = issueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _notificationRepository = notificationRepository;
    }

    public async Task<CreateIssueResponse> ExecuteAsync(CreateIssueCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        int nextNumber = await _issueRepository.GetNextNumberAsync(command.ProjectId, cancellationToken);
        var issue = new Issue(
            command.ProjectId,
            nextNumber,
            command.Title,
            command.Description,
            command.Type,
            command.ReporterId,
            command.Priority,
            command.AssigneeId, 
            command.StoryPoints,
            command.Tags 
        );
        if (issue.AssigneeId.HasValue)
        {
            var notification = Notification.CreateIssueAssigned(
                issue.AssigneeId.Value,
                command.ProjectId,
                issue.Id,
                issue.Title
            );
            await _notificationRepository.AddAsync(notification, cancellationToken);
        }
        await _issueRepository.AddAsync(issue, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateIssueResponse(
            issue.Id,
            issue.Number,
            issue.Title,
            issue.Status,
            issue.ReporterId,
            issue.AssigneeId,
            issue.Tags
        );
    }
}
public class CreateIssueCommandValidator : AbstractValidator<CreateIssueCommand>
{
    public CreateIssueCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Идентификатор проекта обязателен.");

        RuleFor(x => x.ReporterId)
            .NotEmpty().WithMessage("Идентификатор автора (ReporterId) обязателен.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название задачи обязательно.")
            .MaximumLength(250).WithMessage("Название задачи не должно превышать 250 символов.");

        RuleFor(x => x.StoryPoints)
            .GreaterThanOrEqualTo(0).When(x => x.StoryPoints.HasValue)
            .WithMessage("Story Points не могут быть отрицательными.");

        RuleFor(x => x.Tags)
            .NotNull().WithMessage("Список тегов не может быть null.")
            .Must(tags => tags.All(t => !string.IsNullOrWhiteSpace(t)))
            .WithMessage("Теги не могут содержать пустые строки.");
    }
}