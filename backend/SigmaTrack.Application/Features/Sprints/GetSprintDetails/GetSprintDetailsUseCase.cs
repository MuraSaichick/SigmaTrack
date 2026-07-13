using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Exceptions;

namespace SigmaTrack.Application.Features.Sprints.GetSprintDetails;
public record GetSprintDetailsQuery(Guid ProjectId, Guid SprintId);

public record SprintDetailsResponse(
    Guid Id,
    string Name,
    string? Goal,
    string Status,
    DateTime StartDate,
    DateTime EndDate,
    int Capacity,
    int CommittedPoints,
    int CompletedPoints,
    IEnumerable<SprintIssueDto> Issues);

public record SprintIssueDto(
    Guid Id,
    string Key,
    string Title,
    string Status,
    string Type,           
    string Priority,
    int? StoryPoints,
    string? AssigneeName
);
public interface IGetSprintDetailsUseCase
{
    Task<SprintDetailsResponse> ExecuteAsync(GetSprintDetailsQuery query, CancellationToken cancellationToken = default);
}
public interface IGetSprintDetailsRepository
{
    Task<SprintDetailsResponse?> GetDetailsAsync(Guid sprintId, CancellationToken cancellationToken = default);
}
public class GetSprintDetailsUseCase : IGetSprintDetailsUseCase
{
    private readonly ISprintRepository _sprintRepository;

    public GetSprintDetailsUseCase(ISprintRepository sprintRepository)
    {
        _sprintRepository = sprintRepository;
    }

    public async Task<SprintDetailsResponse> ExecuteAsync(GetSprintDetailsQuery query, CancellationToken cancellationToken = default)
    {
        var details = await _sprintRepository.GetDetailsAsync(query.SprintId, cancellationToken);

        if (details == null)
        {
            throw new KeyNotFoundException($"Спринт с ID {query.SprintId} не найден.");
        }
        if (await _sprintRepository.GetProjectIdAsync(query.SprintId, cancellationToken) != query.ProjectId)
        {
            throw new DomainException("Запрошенный спринт не принадлежит указанному проекту.");
        }

        return details;
    }
}