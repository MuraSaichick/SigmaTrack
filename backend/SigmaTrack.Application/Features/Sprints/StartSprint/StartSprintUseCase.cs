using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Exceptions;

namespace SigmaTrack.Application.Features.Sprints.StartSprint;

public record StartSprintCommand(Guid ProjectId, Guid SprintId);
public record StartSprintResponse(Guid SprintId, string Status);

public interface IStartSprintUseCase
{
    Task<StartSprintResponse> ExecuteAsync(StartSprintCommand command, CancellationToken cancellationToken = default);
}
public interface IStartSprintRepository
{
    Task<Domain.Entities.Sprint?> GetByIdAsync(Guid sprintId, CancellationToken cancellationToken = default);
}
public class StartSprintUseCase : IStartSprintUseCase
{
    private readonly ISprintRepository _sprintRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StartSprintUseCase(ISprintRepository sprintRepository, IUnitOfWork unitOfWork)
    {
        _sprintRepository = sprintRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StartSprintResponse> ExecuteAsync(StartSprintCommand command, CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository.GetByIdAsync(command.SprintId, cancellationToken);
        if (sprint == null)
        {
            throw new KeyNotFoundException($"Спринт с ID {command.SprintId} не найден.");
        }
        if (sprint.ProjectId != command.ProjectId)
        {
            throw new DomainException("Указанный спринт не принадлежит данному проекту.");
        }
        sprint.Start();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new StartSprintResponse(sprint.Id, sprint.Status.ToString());
    }
}