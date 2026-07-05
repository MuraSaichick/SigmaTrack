using Microsoft.AspNetCore.Mvc;
using SigmaTrack.Application.Features.Projects.AcceptInvitation;
using SigmaTrack.Application.Features.Projects.CreateInvitation;
using SigmaTrack.Application.Features.Projects.RejectInvitation;
using SigmaTrack.WebApi.Extensions;
using System.Security.Claims;

namespace SigmaTrack.WebApi.Endpoints.Projects;
public class ProjectInvitationEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/projects")
                   .WithTags("Project Invitations")
                   .RequireAuthorization();

        group.MapPost("/{projectId:guid}/invitations", CreateInvitationAsync);

        group.MapPost("/invitations/{invitationId:guid}/accept", AcceptInvitationAsync);

        group.MapPost("/invitations/{invitationId:guid}/reject", RejectInvitationAsync);
    }

    private static async Task<IResult> CreateInvitationAsync(
        [FromRoute] Guid projectId,
        [FromBody] CreateInvitationRequest request,
        [FromServices] ICreateInvitationUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var inviterId = user.GetUserId();

        var command = new CreateInvitationCommand(
            projectId,
            inviterId,
            request.InviteeEmail,
            request.ProjectRoleId
        );
        
        var result = await useCase.ExecuteAsync(command, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> AcceptInvitationAsync(
        [FromRoute] Guid invitationId,
        [FromServices] IAcceptInvitationUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        var command = new AcceptInvitationCommand(invitationId, userId);
        await useCase.ExecuteAsync(command, cancellationToken);

        return Results.Ok(new { message = "Приглашение успешно принято. Вы добавлены в проект." });
    }
    private static async Task<IResult> RejectInvitationAsync(
        [FromRoute] Guid invitationId,
        [FromServices] IRejectInvitationUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        var command = new RejectInvitationCommand(invitationId, userId);
        await useCase.ExecuteAsync(command, cancellationToken);

        return Results.Ok(new { message = "Приглашение отклонено." });
    }
}