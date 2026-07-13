using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Exceptions;

namespace SigmaTrack.Application.Features.Sprints.CompleteSprint;
public record CompleteSprintCommand(Guid ProjectId, Guid SprintId);
public record CompleteSprintResponse(Guid SprintId, string Status, int CompletedPoints);
public interface ICompleteSprintUseCase
{
    Task<CompleteSprintResponse> ExecuteAsync(CompleteSprintCommand command, CancellationToken cancellationToken = default);
}
public interface ICompleteSprintRepository
{
    Task<Domain.Entities.Sprint?> GetWithIssuesByIdAsync(Guid sprintId, CancellationToken cancellationToken = default);
}
public class CompleteSprintUseCase : ICompleteSprintUseCase
{
    private readonly ISprintRepository _sprintRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteSprintUseCase(ISprintRepository sprintRepository, IUnitOfWork unitOfWork)
    {
        _sprintRepository = sprintRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CompleteSprintResponse> ExecuteAsync(CompleteSprintCommand command, CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository.GetWithIssuesByIdAsync(command.SprintId, cancellationToken);
        if (sprint == null)
        {
            throw new KeyNotFoundException($"Спринт с ID {command.SprintId} не найден.");
        }
        if (sprint.ProjectId != command.ProjectId)
        {
            throw new DomainException("Указанный спринт не принадлежит данному проекту.");
        }
        sprint.Complete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CompleteSprintResponse(sprint.Id, sprint.Status.ToString(), sprint.CompletedPoints);
    }
}