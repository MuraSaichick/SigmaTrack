using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SigmaTrack.Application.Features.Notifications.GetNotifications;
using SigmaTrack.Application.Features.Notifications.MarkAsRead;
using SigmaTrack.WebApi.Extensions;

namespace SigmaTrack.WebApi.Endpoints.Notifications
{
    public class NotificationEndpoints : IEndpointModule
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/notifications")
                           .WithTags("Notifications")
                           .RequireAuthorization();

            group.MapGet("/", GetNotificationsAsync)
                 .WithName("GetNotifications")
                 .Produces<IEnumerable<SigmaTrack.Application.Features.Notifications.GetNotifications.NotificationDto>>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status500InternalServerError);
            group.MapPost("/{id:guid}/read", MarkAsReadAsync)
                 .WithName("MarkNotificationAsRead")
                 .Produces(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status403Forbidden)
                 .Produces(StatusCodes.Status400BadRequest);
        }
        private static async Task<IResult> GetNotificationsAsync(
            [FromQuery] bool? isRead,
            [FromServices] IGetNotificationsUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken cancellationToken)
        {
            var userId = user.GetUserId();

            var result = await useCase.ExecuteAsync(userId, isRead, cancellationToken);
            return Results.Ok(result);
        }
        private static async Task<IResult> MarkAsReadAsync(
            [FromRoute] Guid id,
            [FromServices] IMarkAsReadUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken cancellationToken)
        {
            var userId = user.GetUserId();

            await useCase.ExecuteAsync(id, userId, cancellationToken);
            return Results.Ok(new { message = "Уведомление отмечено как прочитанное" });
        }
    }
}