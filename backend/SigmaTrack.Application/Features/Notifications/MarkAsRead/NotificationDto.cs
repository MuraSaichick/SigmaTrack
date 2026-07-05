using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Notifications.MarkAsRead
{
    public record NotificationDto(
    long Id,
    string Type,
    string Title,
    string Message,
    bool IsRead,
    string? LinkUrl,
    DateTimeOffset CreatedAt
);
}
