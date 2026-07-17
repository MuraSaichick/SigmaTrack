using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SigmaTrack.Application.Features.Users.GetUserProfile;
using SigmaTrack.Application.Features.Users.SearchUsers;

namespace SigmaTrack.WebApi.Endpoints.Users;

public class UserEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/users")
           .WithTags("Users")
           .RequireAuthorization();

        group.MapGet("/search", SearchUsersAsync)
            .WithName("SearchUsers")
            .Produces<SearchUsersResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}/profile", GetUserProfileAsync)
            .WithName("GetUserProfile")
            .Produces<UserProfileResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> SearchUsersAsync(
        [AsParameters] SearchUsersRequest request,
        [FromServices] ISearchUsersUseCase useCase,
        CancellationToken cancellationToken)
    {
        var results = await useCase.ExecuteAsync(request, cancellationToken);
        return Results.Ok(results);
    }

    private static async Task<IResult> GetUserProfileAsync(
        [FromRoute] Guid id,
        ClaimsPrincipal user,
        [FromServices] IGetUserProfileUseCase useCase,
        CancellationToken cancellationToken)
    {
        var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Results.Unauthorized();
        }
        var profile = await useCase.ExecuteAsync(currentUserId, id, cancellationToken);
        return Results.Ok(profile);
    }
}