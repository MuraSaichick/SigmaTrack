using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SigmaTrack.Application.Features.Projects.ChangeMemberRole;
using SigmaTrack.Application.Features.Projects.CreateProject;
using SigmaTrack.Application.Features.Projects.DeleteProject;
using SigmaTrack.Application.Features.Projects.GetProjectMembers;
using SigmaTrack.Application.Features.Projects.GetUserProjects;
using SigmaTrack.Application.Features.Projects.LeaveProject;
using SigmaTrack.Application.Features.Projects.RemoveMember;
using SigmaTrack.Application.Features.Projects.UpdateProjectDetails;
using SigmaTrack.WebApi.Extensions;
using System.Security.Claims;

namespace SigmaTrack.WebApi.Endpoints.Projects;

public class ProjectEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/projects")
                       .WithTags("Projects")
                       .RequireAuthorization();

        group.MapPost("/", CreateProjectAsync);
        group.MapGet("/myProjects", GetUserProjectsAsync);

        group.MapGet("/{projectId:guid}/members", GetProjectMembersAsync)
             .WithName("GetProjectMembers")
             .Produces<IEnumerable<ProjectMemberDto>>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status401Unauthorized)
             .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/{projectId:guid}", UpdateProjectDetailsAsync)
             .WithName("UpdateProjectDetails")
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound)
             .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/{projectId:guid}/members/{userId:guid}/role", ChangeMemberRoleAsync)
             .WithName("ChangeMemberRole")
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound)
             .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{projectId:guid}/members/{userId:guid}", RemoveMemberAsync)
             .WithName("RemoveMember")
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status404NotFound)
             .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{projectId:guid}/leave", LeaveProjectAsync)
             .WithName("LeaveProject")
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status401Unauthorized)
             .Produces(StatusCodes.Status404NotFound)
             .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{projectId:guid}", DeleteProjectAsync)
             .WithName("DeleteProject")
             .Produces(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status401Unauthorized)
             .Produces(StatusCodes.Status403Forbidden)
             .Produces(StatusCodes.Status404NotFound)
             .Produces(StatusCodes.Status400BadRequest);
    }
    private static async Task<IResult> CreateProjectAsync(
        [FromBody] CreateProjectRequest request,
        [FromServices] ICreateProjectUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var command = new CreateProjectCommand(
            request.Name,
            request.Description,
            request.Prefix,
            userId
        );

        var result = await useCase.ExecuteAsync(command, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUserProjectsAsync(
        ClaimsPrincipal user,
        [FromServices] IGetUserProjectsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        var response = await useCase.ExecuteAsync(userId, cancellationToken);
        return Results.Ok(response);
    }

    private static async Task<IResult> GetProjectMembersAsync(
        [FromRoute] Guid projectId,
        ClaimsPrincipal user,
        [FromServices] IGetProjectMembersUseCase useCase,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        var members = await useCase.ExecuteAsync(projectId, userId);
        return Results.Ok(members);
    }

    private static async Task<IResult> UpdateProjectDetailsAsync(
        [FromRoute] Guid projectId,
        [FromBody] UpdateProjectDetailsRequest request,
        [FromServices] IUpdateProjectDetailsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProjectDetailsCommand(projectId, request.Name, request.Prefix, request.Description);
        await useCase.ExecuteAsync(command, cancellationToken);

        return Results.Ok(new { message = "Информация о проекте успешно обновлена." });
    }

    private static async Task<IResult> ChangeMemberRoleAsync(
        [FromRoute] Guid projectId,
        [FromRoute] Guid userId,
        [FromBody] ChangeMemberRoleRequest request,
        [FromServices] IChangeMemberRoleUseCase useCase,
        CancellationToken cancellationToken)
    {
        var command = new ChangeMemberRoleCommand(projectId, userId, request.NewRole);
        await useCase.ExecuteAsync(command, cancellationToken);

        return Results.Ok(new { message = "Роль участника успешно изменена." });
    }

    private static async Task<IResult> RemoveMemberAsync(
        [FromRoute] Guid projectId,
        [FromRoute] Guid userId,
        [FromServices] IRemoveMemberUseCase useCase,
        CancellationToken cancellationToken)
    {
        var command = new RemoveMemberCommand(projectId, userId);
        await useCase.ExecuteAsync(command, cancellationToken);
        return Results.Ok(new { message = "Участник успешно удален из проекта." });
    }

    private static async Task<IResult> LeaveProjectAsync(
        [FromRoute] Guid projectId,
        [FromServices] ILeaveProjectUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        var command = new LeaveProjectCommand(projectId, userId);
        await useCase.ExecuteAsync(command, cancellationToken);

        return Results.Ok(new { message = "Вы успешно покинули проект." });
    }

    private static async Task<IResult> DeleteProjectAsync(
        [FromRoute] Guid projectId,
        [FromServices] IDeleteProjectUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        var command = new DeleteProjectCommand(projectId, userId);
        await useCase.ExecuteAsync(command, cancellationToken);

        return Results.Ok(new { message = "Проект успешно удален." });
    }
}