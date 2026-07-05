using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SigmaTrack.Application.Features.Issues.AddComment;
using SigmaTrack.Application.Features.Issues.AssignIssue;
using SigmaTrack.Application.Features.Issues.ChangeStatus;
using SigmaTrack.Application.Features.Issues.CreateIssue;
using SigmaTrack.Application.Features.Issues.GetIssueById;
using SigmaTrack.Application.Features.Issues.GetIssuesList;
using SigmaTrack.Application.Features.Issues.GetUserActiveIssues;
using SigmaTrack.Application.Features.Issues.UpdateIssueDetails;
using SigmaTrack.WebApi.Extensions;
using System.Security.Claims;

namespace SigmaTrack.WebApi.Endpoints.Issues;

public class IssueEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/issues")
                       .WithTags("Issues")
                       .RequireAuthorization();

        group.MapPost("/", CreateIssueAsync);
        group.MapGet("/", GetIssuesAsync);
        group.MapGet("/{id:guid}", GetIssueByIdAsync);
        group.MapPost("/{id:guid}/comments", AddCommentAsync);

        group.MapPut("/{issueId:guid}/status", ChangeStatusAsync)
            .WithName("ChangeIssueStatus")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/{issueId:guid}", UpdateIssueDetailsAsync)
            .WithName("UpdateIssueDetails")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}/assignee", AssignIssueAsync)
            .WithName("AssignIssue"); // Лишний .RequireAuthorization() и .WithTags() убраны, т.к. они наследуются от группы[cite: 1]

        group.MapGet("/users/{userId:guid}/active-issues", GetActiveIssuesByAssigneeAsync)
            .WithName("GetActiveIssuesByAssignee")
            .Produces<IReadOnlyCollection<UserActiveIssueResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> CreateIssueAsync(
        [FromBody] CreateIssueRequest request,
        [FromServices] ICreateIssueUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        var command = new CreateIssueCommand(
            request.ProjectId,
            userId,
            request.Title,
            request.Description,
            request.Type,
            request.Priority,
            request.AssigneeId,
            request.StoryPoints,
            request.Tags ?? new List<string>()
        );

        var result = await useCase.ExecuteAsync(command, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetIssuesAsync(
        [AsParameters] GetIssuesListQuery query,
        [FromServices] IGetIssuesListUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetIssueByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IGetIssueByIdUseCase useCase,
        CancellationToken cancellationToken)
    {
        var query = new GetIssueByIdQuery(id);
        var result = await useCase.ExecuteAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> AddCommentAsync(
        [FromRoute] Guid id,
        [FromBody] AddCommentRequest request,
        [FromServices] IAddCommentUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        var command = new AddCommentCommand(
            IssueId: id,
            AuthorId: userId,
            Text: request.Text,
            IsInternal: request.IsInternal,
            Mentions: request.Mentions ?? new List<string>(),
            Attachments: request.Attachments ?? new List<AttachmentInput>()
        );

        var result = await useCase.ExecuteAsync(command, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ChangeStatusAsync(
        [FromRoute] Guid issueId,
        [FromBody] ChangeStatusRequest request,
        [FromServices] IChangeStatusUseCase useCase,
        CancellationToken cancellationToken)
    {
        var command = new ChangeStatusCommand(issueId, request.NewStatus);
        await useCase.ExecuteAsync(command, cancellationToken);

        return Results.Ok(new { message = "Статус задачи успешно обновлен." });
    }

    private static async Task<IResult> UpdateIssueDetailsAsync(
         [FromRoute] Guid issueId,
         [FromBody] UpdateIssueRequest request,
         [FromServices] IUpdateIssueDetailsUseCase useCase,
         CancellationToken cancellationToken)
    {
        var command = new UpdateIssueDetailsCommand(
            IssueId: issueId,
            Title: request.Title,
            Description: request.Description,
            Priority: request.Priority,
            Severity: request.Severity,
            DueDate: request.DueDate,
            EstimatedHours: request.EstimatedHours,
            RemainingHours: request.RemainingHours,
            Component: request.Component,
            Version: request.Version,
            FixVersion: request.FixVersion,
            Environment: request.Environment,
            Tags: request.Tags,
            IsReproducible: request.IsReproducible,
            StepsToReproduce: request.StepsToReproduce
        );

        await useCase.ExecuteAsync(command, cancellationToken);
        return Results.Ok(new { message = "Данные задачи успешно обновлены." });
    }

    private static async Task<IResult> AssignIssueAsync(
        [FromRoute] Guid id,
        [FromBody] AssignIssueRequest request,
        [FromServices] IAssignIssueUseCase useCase,
        ClaimsPrincipal user, // HttpContext заменен на ClaimsPrincipal[cite: 1]
        CancellationToken cancellationToken)
    {
        var currentUserId = user.GetUserId();

        var command = new AssignIssueCommand(id, request.AssigneeId, currentUserId);
        await useCase.ExecuteAsync(command, cancellationToken);

        return Results.Ok(new { Message = "Исполнитель задачи успешно изменен." });
    }

    private static async Task<IResult> GetActiveIssuesByAssigneeAsync(
        [FromRoute] Guid userId,
        [FromServices] IGetUserActiveIssuesUseCase useCase,
        CancellationToken cancellationToken)
    {
        var issues = await useCase.ExecuteAsync(userId, cancellationToken);
        return Results.Ok(issues);
    }
}