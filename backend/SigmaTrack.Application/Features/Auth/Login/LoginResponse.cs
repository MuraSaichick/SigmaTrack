using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Auth.Login
{
    public record LoginResponse(Guid Id, string Token, string Login, string Email, string Firstname, string Lastname, string? AvatarUrl);
}
