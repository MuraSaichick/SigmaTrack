using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Auth.Register
{
    public record RegisterRequest(
    string Login,
    string Email,
    string Phone,
    string Password,
    string Firstname,
    string Lastname,
    string? Patronymic
);
}
