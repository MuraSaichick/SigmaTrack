namespace SigmaTrack.Application.Features.Issues.ChangeStatus;

using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Enums;
using SigmaTrack.Domain.Exceptions;
public record ChangeStatusCommand(Guid IssueId, IssueStatus NewStatus);
public record ChangeStatusRequest(IssueStatus NewStatus);
public interface IChangeStatusUseCase
{
    Task ExecuteAsync(ChangeStatusCommand command, CancellationToken cancellationToken);
}

public class ChangeStatusUseCase : IChangeStatusUseCase
{
    private readonly IIssueRepository _issueRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeStatusUseCase(IIssueRepository issueRepository, IUnitOfWork unitOfWork)
    {
        _issueRepository = issueRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(ChangeStatusCommand command, CancellationToken cancellationToken)
    {
        if (command.IssueId == Guid.Empty)
            throw new ArgumentException("Идентификатор задачи не может быть пустым.");
        var issue = await _issueRepository.GetByIdAsync(command.IssueId, cancellationToken);
        if (issue == null)
            throw new KeyNotFoundException($"Задача с идентификатором {command.IssueId} не найдена.");
        issue.ChangeStatus(command.NewStatus);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}