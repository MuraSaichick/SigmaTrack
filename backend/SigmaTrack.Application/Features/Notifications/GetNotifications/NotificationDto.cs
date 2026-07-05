using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Notifications.GetNotifications
{
    public record NotificationDto(
        Guid Id,
        NotificationType Type,
        string Title,
        string Message,
        bool IsRead,
        string LinkUrl,
        DateTimeOffset CreatedAt
        );
}