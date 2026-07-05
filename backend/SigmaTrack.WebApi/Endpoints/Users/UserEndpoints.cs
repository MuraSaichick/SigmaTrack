using Microsoft.AspNetCore.Mvc;
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
            .Produces<List<UserSearchResultDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
    }
    private static async Task<IResult> SearchUsersAsync(
        [FromQuery] string query,
        [FromServices] ISearchUsersUseCase useCase,
        CancellationToken cancellationToken,
        [FromQuery] int limit = 10)
    {
        var results = await useCase.ExecuteAsync(query, cancellationToken);
        return Results.Ok(results);
    }
}