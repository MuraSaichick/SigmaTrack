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
        var projectGroup = app.MapGroup("api/projects/{projectId:guid}/issues")
                              .WithTags("Issues")
                              .RequireAuthorization();

        projectGroup.MapPost("", CreateIssueAsync);
        projectGroup.MapGet("", GetIssuesAsync);

        var issueGroup = app.MapGroup("api/issues")
                            .WithTags("Issues")
                            .RequireAuthorization();

        issueGroup.MapGet("/{id:guid}", GetIssueByIdAsync);
        issueGroup.MapPost("/{id:guid}/comments", AddCommentAsync);

        issueGroup.MapPut("/{issueId:guid}/status", ChangeStatusAsync)
            .WithName("ChangeIssueStatus")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        issueGroup.MapPut("/{issueId:guid}", UpdateIssueDetailsAsync)
            .WithName("UpdateIssueDetails")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        issueGroup.MapPut("/{id:guid}/assignee", AssignIssueAsync)
            .WithName("AssignIssue");

        issueGroup.MapGet("/users/{userId:guid}/active-issues", GetActiveIssuesByAssigneeAsync)
            .WithName("GetActiveIssuesByAssignee");
    }
    private static async Task<IResult> CreateIssueAsync(
        [FromRoute] Guid projectId,
        [FromBody] CreateIssueRequest request,
        [FromServices] ICreateIssueUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {   
        var userId = user.GetUserId();

        var command = new CreateIssueCommand(
            projectId,
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
        [FromRoute] Guid projectId,
        [AsParameters] GetIssuesApiRequest apiRequest,
        [FromServices] IGetIssuesListUseCase useCase,
        CancellationToken cancellationToken)
    {
        var query = new GetIssuesListQuery(
            ProjectId: projectId,
            Status: apiRequest.Status,
            Type: apiRequest.Type,
            Priority: apiRequest.Priority,
            AssigneeId: apiRequest.AssigneeId,
            SearchTerm: apiRequest.SearchTerm,
            PageNumber: apiRequest.PageNumber,
            PageSize: apiRequest.PageSize
         );

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
            StepsToReproduce: request.StepsToReproduce);

        await useCase.ExecuteAsync(command, cancellationToken);
        return Results.Ok(new { message = "Данные задачи успешно обновлены." });
    }

    private static async Task<IResult> AssignIssueAsync(
        [FromRoute] Guid id,
        [FromBody] AssignIssueRequest request,
        [FromServices] IAssignIssueUseCase useCase,
        ClaimsPrincipal user,
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