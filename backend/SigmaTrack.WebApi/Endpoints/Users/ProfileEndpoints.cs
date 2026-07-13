using Microsoft.AspNetCore.Mvc;
using SigmaTrack.Application.Features.Profile.GetProfile;
using SigmaTrack.Application.Features.Profile.UpdateProfile;
using SigmaTrack.Application.Features.Profile.UploadAvatar;
using SigmaTrack.WebApi.Extensions;
using System.Security.Claims;

namespace SigmaTrack.WebApi.Endpoints.Users;

public class ProfileEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/profile")
            .WithTags("User Profile")
            .RequireAuthorization();

        group.MapPut("/", UpdateProfileAsync)
            .WithName("UpdateProfile")
            .Produces<UserProfileResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/", GetProfileAsync)
            .WithName("GetProfile")
            .Produces<UserProfileResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
        group.MapPost("/avatar", UploadAvatarAsync)
            .WithName("UploadAvatar")
            .DisableAntiforgery()
            .Produces<UploadAvatarResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetProfileAsync(
        [FromServices] IGetProfileUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        var result = await useCase.ExecuteAsync(userId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateProfileAsync(
        [FromBody] UpdateProfileRequest request,
        [FromServices] IUpdateProfileUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        var command = new UpdateProfileCommand(
            userId,
            request.Firstname,
            request.Lastname,
            request.Patronymic,
            request.Phone,
            request.StatusMessage,
            request.Bio,
            request.Position,
            request.Department,
            request.Skills,
            request.BirthDate,
            request.Telegram,
            request.GitHub
        );

        var result = await useCase.ExecuteAsync(command, cancellationToken);
        return Results.Ok(result);
    }
    private static async Task<IResult> UploadAvatarAsync(
        IFormFile file,
        [FromServices] IUploadAvatarUseCase useCase,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        using var stream = file.OpenReadStream();
        var request = new UploadAvatarRequest(
            UserId: userId,
            FileStream: stream,
            FileName: file.FileName,
            ContentType: file.ContentType
        );
        var result = await useCase.ExecuteAsync(request, cancellationToken);
        return Results.Ok(result);
    }
}