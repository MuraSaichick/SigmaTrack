using FluentValidation;
using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Enums;

namespace SigmaTrack.Application.Features.Issues.UpdateIssueDetails;
public record UpdateIssueRequest(
    string Title,
    string? Description,
    IssuePriority Priority,
    IssueSeverity? Severity,
    DateTime? DueDate,
    double? EstimatedHours,
    double? RemainingHours,
    string? Component,
    string? Version,
    string? FixVersion,
    string? Environment,
    List<string> Tags,
    bool IsReproducible,
    string? StepsToReproduce
);
public record UpdateIssueDetailsCommand(
    Guid IssueId,
    string Title,
    string? Description,
    IssuePriority Priority,
    IssueSeverity? Severity,
    DateTime? DueDate,
    double? EstimatedHours,
    double? RemainingHours,
    string? Component,
    string? Version,
    string? FixVersion,
    string? Environment,
    List<string> Tags,
    bool IsReproducible,
    string? StepsToReproduce
);
public interface IUpdateIssueDetailsUseCase
{
    Task ExecuteAsync(UpdateIssueDetailsCommand command, CancellationToken cancellationToken);
}

public class UpdateIssueDetailsUseCase : IUpdateIssueDetailsUseCase
{
    private readonly IIssueRepository _issueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateIssueDetailsCommand> _validator;

    public UpdateIssueDetailsUseCase(
        IIssueRepository issueRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateIssueDetailsCommand> validator)
    {
        _issueRepository = issueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task ExecuteAsync(UpdateIssueDetailsCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        var issue = await _issueRepository.GetByIdAsync(command.IssueId, cancellationToken);
        if (issue == null)
        {
            throw new KeyNotFoundException($"Задача с идентификатором {command.IssueId} не найдена.");
        }
        issue.UpdateDetails(
            command.Title,
            command.Description,
            command.Priority,
            command.Severity,
            command.DueDate,
            command.EstimatedHours,
            command.RemainingHours,
            command.Component,
            command.Version,
            command.FixVersion,
            command.Environment,
            command.Tags,
            command.IsReproducible,
            command.StepsToReproduce
        );
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
public class UpdateIssueDetailsCommandValidator : AbstractValidator<UpdateIssueDetailsCommand>
{
    public UpdateIssueDetailsCommandValidator()
    {
        RuleFor(x => x.IssueId).NotEmpty().WithMessage("Идентификатор задачи обязателен.");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(250).WithMessage("Название задачи обязательно и не должно превышать 250 символов.");
        RuleFor(x => x.EstimatedHours).GreaterThanOrEqualTo(0).When(x => x.EstimatedHours.HasValue).WithMessage("Плановые часы не могут быть отрицательными.");
        RuleFor(x => x.RemainingHours).GreaterThanOrEqualTo(0).When(x => x.RemainingHours.HasValue).WithMessage("Оставшиеся часы не могут быть отрицательными.");
    }
}