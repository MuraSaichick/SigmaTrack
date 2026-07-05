using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Profile.UpdateProfile
{
    public record UserProfileResponse(
    Guid Id,
    string Login,
    string Firstname,
    string Lastname,
    string? Patronymic,
    string Email,
    string Phone,
    string? AvatarUrl,
    string? AvatarColor,
    string? StatusMessage,
    string? Bio,
    string? Position,
    string? Department,
    List<string> Skills,
    DateTime? BirthDate,
    string? Telegram,
    string? GitHub
);
}
