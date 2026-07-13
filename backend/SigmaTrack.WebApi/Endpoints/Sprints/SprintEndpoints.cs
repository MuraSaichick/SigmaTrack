using Microsoft.AspNetCore.Mvc;
using SigmaTrack.Application.Features.Sprints;
using SigmaTrack.Application.Features.Sprints.AddIssuesToSprint;
using SigmaTrack.Application.Features.Sprints.CancelSprint;
using SigmaTrack.Application.Features.Sprints.CompleteSprint;
using SigmaTrack.Application.Features.Sprints.GetProjectSprints;
using SigmaTrack.Application.Features.Sprints.GetSprintDetails;
using SigmaTrack.Application.Features.Sprints.RemoveIssueFromSprint;
using SigmaTrack.Application.Features.Sprints.StartSprint;
using SigmaTrack.Domain.Enums;

namespace SigmaTrack.WebApi.Endpoints.Sprints;

public class SprintEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/projects/{projectId:guid}/sprints")
                       .WithTags("Sprints")
                       .RequireAuthorization();
        group.MapPost("/", CreateSprintAsync)
             .WithName("CreateSprint")
             .Produces<CreateSprintResponse>(StatusCodes.Status201Created)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status401Unauthorized)
             .Produces(StatusCodes.Status404NotFound)
             .Produces(StatusCodes.Status500InternalServerError);
        group.MapPut("/{sprintId:guid}/start", StartSprintAsync)
             .WithName("StartSprint")
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);
        group.MapPut("/{sprintId:guid}/complete", CompleteSprintAsync)
             .WithName("CompleteSprint")
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);
        group.MapPost("/{sprintId:guid}/issues", AddIssuesToSprintAsync)
             .WithName("AddIssuesToSprint")
             .Produces<AddIssuesToSprintResponse>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);
        group.MapGet("/", GetProjectSprintsAsync)
             .WithName("GetProjectSprints")
             .Produces<IEnumerable<SprintLookupDto>>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status401Unauthorized)
             .Produces(StatusCodes.Status404NotFound)
             .Produces(StatusCodes.Status500InternalServerError);
        group.MapGet("/{sprintId:guid}", GetSprintDetailsAsync)
             .WithName("GetSprintDetails")
             .Produces<SprintDetailsResponse>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status401Unauthorized)
             .Produces(StatusCodes.Status404NotFound)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status500InternalServerError);
        group.MapDelete("/{sprintId:guid}/issues/{issueId:guid}", RemoveIssueFromSprintAsync)
             .WithName("RemoveIssueFromSprint")
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound);
        group.MapPut("/{sprintId:guid}/cancel", CancelSprintAsync)
                 .WithName("CancelSprint")
                 .Produces(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status400BadRequest)
                 .Produces(StatusCodes.Status404NotFound);
    }
    private static async Task<IResult> CreateSprintAsync(
        [FromRoute] Guid projectId,
        [FromBody] CreateSprintApiRequest apiRequest,
        [FromServices] ICreateSprintUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new CreateSprintRequest(
            projectId,
            apiRequest.Name,
            apiRequest.Goal,
            apiRequest.StartDate,
            apiRequest.EndDate,
            apiRequest.Capacity
        );

        var response = await useCase.ExecuteAsync(request, cancellationToken);

        return Results.Created($"/api/projects/{projectId}/sprints/{response.Id}", response);
    }

    private static async Task<IResult> StartSprintAsync(
     [FromRoute] Guid projectId,
     [FromRoute] Guid sprintId,
     [FromServices] IStartSprintUseCase useCase,
     CancellationToken cancellationToken)
    {
        var command = new StartSprintCommand(projectId, sprintId);
        var response = await useCase.ExecuteAsync(command, cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<IResult> CompleteSprintAsync(
        [FromRoute] Guid projectId,
        [FromRoute] Guid sprintId,
        [FromServices] ICompleteSprintUseCase useCase,
        CancellationToken cancellationToken)
    {
        var command = new CompleteSprintCommand(projectId, sprintId);
        var response = await useCase.ExecuteAsync(command, cancellationToken);

        return Results.Ok(response);
    }
    private static async Task<IResult> AddIssuesToSprintAsync(
        [FromRoute] Guid projectId,
        [FromRoute] Guid sprintId,
        [FromBody] AddIssuesApiRequest apiRequest,
        [FromServices] IAddIssuesToSprintUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new AddIssuesToSprintRequest(projectId, sprintId, apiRequest.IssueIds);
        var response = await useCase.ExecuteAsync(request, cancellationToken);

        return Results.Ok(response);
    }
    private static async Task<IResult> GetProjectSprintsAsync(
    [FromRoute] Guid projectId,
    [FromQuery] SprintStatus? status,
    [FromServices] IGetProjectSprintsUseCase useCase,
    CancellationToken cancellationToken)
    {
        var query = new GetProjectSprintsQuery(projectId, status);

        var result = await useCase.ExecuteAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetSprintDetailsAsync(
        [FromRoute] Guid projectId,
        [FromRoute] Guid sprintId,
        [FromServices] IGetSprintDetailsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var query = new GetSprintDetailsQuery(projectId, sprintId);

        var result = await useCase.ExecuteAsync(query, cancellationToken);
        return Results.Ok(result);
    }
    private static async Task<IResult> CancelSprintAsync(
        [FromRoute] Guid projectId,
        [FromRoute] Guid sprintId,
        [FromServices] ICancelSprintUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(projectId, sprintId, cancellationToken);

        return Results.Ok(new { message = "Спринт успешно отменен. Незавершенные задачи возвращены в бэклог." });
    }
    private static async Task<IResult> RemoveIssueFromSprintAsync(
        [FromRoute] Guid projectId,
        [FromRoute] Guid sprintId,
        [FromRoute] Guid issueId,
        [FromServices] IRemoveIssueFromSprintUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(projectId, sprintId, issueId, cancellationToken);

        return Results.Ok(new { message = "Задача успешно удалена из спринта и возвращена в бэклог." });
    }
}
public record CreateSprintApiRequest(
    string Name,
    string? Goal,
    DateTime StartDate,
    DateTime EndDate,
    int Capacity);

public record AddIssuesApiRequest(List<Guid> IssueIds);