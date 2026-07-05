using System.Security.Claims;

namespace SigmaTrack.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException("Пользователь не авторизован или ID некорректен.");
        }
        return userId;
    }
}