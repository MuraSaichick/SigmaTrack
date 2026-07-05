using FluentValidation;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Application.Interfaces;

namespace SigmaTrack.Application.Features.Issues.AssignIssue;

public record AssignIssueRequest(Guid? AssigneeId);
public record AssignIssueCommand(Guid IssueId, Guid? AssigneeId, Guid CurrentUserId);
public class AssignIssueCommandValidator : AbstractValidator<AssignIssueCommand>
{
    public AssignIssueCommandValidator()
    {
        RuleFor(x => x.IssueId)
            .NotEmpty().WithMessage("Идентификатор задачи обязателен.");
    }
}
public interface IAssignIssueUseCase
{
    Task ExecuteAsync(AssignIssueCommand command, CancellationToken cancellationToken);
}
public class AssignIssueUseCase : IAssignIssueUseCase
{
    private readonly IIssueRepository _issueRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AssignIssueCommand> _validator;
    public AssignIssueUseCase(
        IIssueRepository issueRepository,
        IProjectRepository projectRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        IValidator<AssignIssueCommand> validator)
    {
        _issueRepository = issueRepository;
        _projectRepository = projectRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }
    public async Task ExecuteAsync(AssignIssueCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        var issue = await _issueRepository.GetByIdAsync(command.IssueId, cancellationToken);
        if (issue == null)
        {
            throw new KeyNotFoundException($"Задача с ID {command.IssueId} не найдена.");
        }
        if (command.AssigneeId.HasValue)
        {
            var isMember = await _projectRepository.IsProjectMemberAsync(issue.ProjectId, command.AssigneeId.Value, cancellationToken);
            if (!isMember)
            {
                throw new InvalidOperationException("Указанный пользователь не является участником этого проекта.");
            }
        }
        bool isChanged = issue.AssignTo(command.AssigneeId);
        if (isChanged && command.AssigneeId.HasValue && command.AssigneeId.Value != command.CurrentUserId)
        {
            var notification = Notification.CreateIssueAssigned(
                command.AssigneeId.Value,
                issue.ProjectId,
                issue.Id,
                issue.Title
            );
            await _notificationRepository.AddAsync(notification, cancellationToken);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}