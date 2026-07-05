using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Auth.Login
{
    public record LoginRequest(string Login, string Password);
}
