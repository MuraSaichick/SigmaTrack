using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Exceptions;

namespace SigmaTrack.Application.Features.Sprints.CancelSprint;

public interface ICancelSprintUseCase
{
    Task<bool> ExecuteAsync(Guid projectId, Guid sprintId, CancellationToken cancellationToken = default);
}

public class CancelSprintUseCase : ICancelSprintUseCase
{
    private readonly ISprintRepository _sprintRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSprintUseCase(ISprintRepository sprintRepository, IUnitOfWork unitOfWork)
    {
        _sprintRepository = sprintRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(Guid projectId, Guid sprintId, CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository.GetWithIssuesByIdAsync(sprintId, cancellationToken);
        if (sprint == null)
        {
            throw new KeyNotFoundException($"Спринт с ID {sprintId} не найден.");
        }
        if (sprint.ProjectId != projectId)
        {
            throw new DomainException("Указанный спринт не принадлежит данному проекту.");
        }
        sprint.Cancel();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}