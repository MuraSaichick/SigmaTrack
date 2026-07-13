using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Exceptions;

namespace SigmaTrack.Application.Features.Sprints.RemoveIssueFromSprint;

public interface IRemoveIssueFromSprintUseCase
{
    Task<bool> ExecuteAsync(Guid projectId, Guid sprintId, Guid issueId, CancellationToken cancellationToken = default);
}

public class RemoveIssueFromSprintUseCase : IRemoveIssueFromSprintUseCase
{
    private readonly ISprintRepository _sprintRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveIssueFromSprintUseCase(ISprintRepository sprintRepository, IUnitOfWork unitOfWork)
    {
        _sprintRepository = sprintRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(Guid projectId, Guid sprintId, Guid issueId, CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository.GetWithIssuesByIdAsync(sprintId, cancellationToken);
        if (sprint == null)
        {
            throw new KeyNotFoundException($"Спринт с ID {sprintId} не найден.");
        }
        if (sprint.ProjectId != projectId)
        {
            throw new DomainException("Спринт не принадлежит указанному проекту.");
        }
        var issue = sprint.Issues.FirstOrDefault(i => i.Id == issueId);
        if (issue == null)
        {
            throw new KeyNotFoundException($"Задача с ID {issueId} не найдена в этом спринте.");
        }
        sprint.RemoveIssue(issue);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}