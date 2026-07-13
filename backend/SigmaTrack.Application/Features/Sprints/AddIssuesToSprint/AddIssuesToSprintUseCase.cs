using FluentValidation;
using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Exceptions;

namespace SigmaTrack.Application.Features.Sprints.AddIssuesToSprint;
public record AddIssuesToSprintRequest(
    Guid ProjectId,
    Guid SprintId,
    List<Guid> IssueIds);

public record AddIssuesToSprintResponse(
    Guid SprintId,
    int TotalIssuesAdded,
    int NewCommittedPoints);
public interface IAddIssuesToSprintUseCase
{
    Task<AddIssuesToSprintResponse> ExecuteAsync(AddIssuesToSprintRequest request, CancellationToken cancellationToken = default);
}
public interface IAddIssuesToSprintRepository
{
    Task<List<Domain.Entities.Issue>> GetIssuesByIdsAsync(Guid projectId, List<Guid> issueIds, CancellationToken cancellationToken = default);
}
public class AddIssuesToSprintUseCase : IAddIssuesToSprintUseCase
{
    private readonly ISprintRepository _sprintRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AddIssuesToSprintRequest> _validator;

    public AddIssuesToSprintUseCase(
        ISprintRepository sprintRepository,
        IIssueRepository issueRepository,
        IUnitOfWork unitOfWork,
        IValidator<AddIssuesToSprintRequest> validator)
    {
        _sprintRepository = sprintRepository;
        _issueRepository = issueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<AddIssuesToSprintResponse> ExecuteAsync(AddIssuesToSprintRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        var sprint = await _sprintRepository.GetWithIssuesByIdAsync(request.SprintId, cancellationToken);
        if (sprint == null)
            throw new KeyNotFoundException($"Спринт с ID {request.SprintId} не найден.");
        if (sprint.ProjectId != request.ProjectId)
            throw new DomainException("Указанный спринт не принадлежит данному проекту.");
        var issues = await _issueRepository.GetIssuesByIdsAsync(request.ProjectId, request.IssueIds, cancellationToken);

        if (issues.Count != request.IssueIds.Distinct().Count())
            throw new DomainException("Некоторые из указанных задач не найдены или принадлежат другому проекту.");
        foreach (var issue in issues)
        {
            issue.AssignToSprint(sprint);
        }
        sprint.RecalculateCommittedPoints();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AddIssuesToSprintResponse(sprint.Id, issues.Count, sprint.CommittedPoints);
    }
}
public class AddIssuesToSprintRequestValidator : AbstractValidator<AddIssuesToSprintRequest>
{
    public AddIssuesToSprintRequestValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.SprintId).NotEmpty();
        RuleFor(x => x.IssueIds).NotEmpty().WithMessage("Список задач не может быть пустым.");
        RuleFor(x => x.IssueIds.Count).LessThanOrEqualTo(100).WithMessage("Нельзя добавить более 100 задач за один раз.");
    }
}