using Microsoft.AspNetCore.Mvc;
using SigmaTrack.Application.Features.UserSettings.ChangeEmail;
using SigmaTrack.Application.Features.UserSettings.ChangePassword;
using SigmaTrack.Application.Features.UserSettings.GetPrivacy;
using SigmaTrack.Application.Features.UserSettings.UpdatePrivacy;
using SigmaTrack.WebApi.Extensions;
using System.Security.Claims;

namespace SigmaTrack.WebApi.Endpoints.Users;

public class UserSettingsEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/settings")
            .WithTags("User Settings")
            .RequireAuthorization();

        group.MapPut("/privacy", UpdatePrivacyAsync)
            .WithName("UpdatePrivacy")
            .Produces<UpdatePrivacyResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        group.MapPost("/change-password", ChangePasswordAsync)
            .WithName("ChangePassword")
            .Produces<ChangePasswordResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
        group.MapPost("/change-email", ChangeEmailAsync)
            .WithName("ChangeEmail")
            .Produces<ChangeEmailResponse>(StatusCodes.Status200OK)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
        group.MapGet("/privacy", GetPrivacyAsync)
            .WithName("GetPrivacy")
            .Produces<PrivacySettingsResponse>(StatusCodes.Status200OK);
    }
    private static async Task<IResult> UpdatePrivacyAsync(
        [FromBody] UpdatePrivacyRequest request,
        [FromServices] IUpdatePrivacyUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var result = await useCase.ExecuteAsync(userId, request, cancellationToken);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
    private static async Task<IResult> ChangePasswordAsync(
        [FromBody] ChangePasswordRequest request,
        [FromServices] IChangePasswordUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var result = await useCase.ExecuteAsync(userId, request, cancellationToken);
        return Results.Ok(result);
    }
    private static async Task<IResult> ChangeEmailAsync(
        [FromBody] ChangeEmailRequest request,
        [FromServices] IChangeEmailUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var result = await useCase.ExecuteAsync(userId, request, cancellationToken);
        return Results.Ok(result);
    }
    private static async Task<IResult> GetPrivacyAsync(
    [FromServices] IGetPrivacySettingsUseCase useCase,
    ClaimsPrincipal user,
    CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var result = await useCase.ExecuteAsync(userId, cancellationToken);
        return Results.Ok(result);
    }
}