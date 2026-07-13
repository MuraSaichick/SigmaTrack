using FluentValidation;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Enums;

namespace SigmaTrack.Application.Features.Sprints.GetProjectSprints;
public record GetProjectSprintsQuery(
    Guid ProjectId,
    SprintStatus? Status);

public record SprintLookupDto(
    Guid Id,
    string Name,
    string? Goal,
    string Status,
    DateTime StartDate,
    DateTime EndDate,
    int Capacity,
    int CommittedPoints);
public interface IGetProjectSprintsUseCase
{
    Task<IEnumerable<SprintLookupDto>> ExecuteAsync(GetProjectSprintsQuery query, CancellationToken cancellationToken = default);
}
public interface IGetProjectSprintsRepository
{
    Task<IEnumerable<SprintLookupDto>> GetByProjectWithFiltersAsync(Guid projectId, SprintStatus? status, CancellationToken cancellationToken = default);
}
public class GetProjectSprintsUseCase : IGetProjectSprintsUseCase
{
    private readonly ISprintRepository _sprintRepository;
    private readonly IProjectRepository _projectRepository;
    public GetProjectSprintsUseCase(ISprintRepository sprintRepository, IProjectRepository projectRepository)
    {
        _sprintRepository = sprintRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<SprintLookupDto>> ExecuteAsync(GetProjectSprintsQuery query, CancellationToken cancellationToken = default)
    {
        if (!await _projectRepository.ExistsAsync(query.ProjectId, cancellationToken))
        {
            throw new KeyNotFoundException($"Проект с ID {query.ProjectId} не найден.");
        }

        return await _sprintRepository.GetByProjectWithFiltersAsync(query.ProjectId, query.Status, cancellationToken);
    }
}