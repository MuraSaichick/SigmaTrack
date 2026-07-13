using FluentValidation;
using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;

namespace SigmaTrack.Application.Features.Sprints;
public record CreateSprintRequest(
    Guid ProjectId,
    string Name,
    string? Goal,
    DateTime StartDate,
    DateTime EndDate,
    int Capacity);

public record CreateSprintResponse(
    Guid Id,
    string Name,
    SprintStatus Status,
    DateTime StartDate,
    DateTime EndDate);
public interface ICreateSprintUseCase
{
    Task<CreateSprintResponse> ExecuteAsync(CreateSprintRequest request, CancellationToken cancellationToken = default);
}
public class CreateSprintUseCase : ICreateSprintUseCase
{
    private readonly ISprintRepository _sprintRepository;
    private readonly IProjectRepository _projectRepository;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateSprintRequest> _validator;

    public CreateSprintUseCase(
        ISprintRepository sprintRepository,
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateSprintRequest> validator)
    {
        _sprintRepository = sprintRepository;
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CreateSprintResponse> ExecuteAsync(CreateSprintRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        var projectExists = await _projectRepository.ExistsAsync(request.ProjectId, cancellationToken);
        if (!projectExists)
        {
            throw new KeyNotFoundException($"Проект с ID {request.ProjectId} не найден.");
        }
        var sprint = new Sprint(
            request.ProjectId,
            request.Name,
            request.Goal,
            request.StartDate,
            request.EndDate,
            request.Capacity
        );

        await _sprintRepository.AddAsync(sprint, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateSprintResponse(
            sprint.Id,
            sprint.Name,
            sprint.Status,
            sprint.StartDate,
            sprint.EndDate
        );
    }
}
public class CreateSprintRequestValidator : AbstractValidator<CreateSprintRequest>
{
    public CreateSprintRequestValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty().WithMessage("ProjectId обязателен.");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150).WithMessage("Название спринта обязательно и не должно превышать 150 символов.");
        RuleFor(x => x.Capacity).GreaterThan(0).WithMessage("Емкость спринта должна быть больше 0.");
        RuleFor(x => x.StartDate).NotEmpty().WithMessage("Дата начала обязательна.");
        RuleFor(x => x.EndDate).NotEmpty().WithMessage("Дата окончания обязательна.");
    }
}