using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Profile.UpdateProfile
{
    public record UpdateProfileCommand(
        Guid UserId,
        string Firstname,
        string Lastname,
        string? Patronymic,
        string Phone,
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
