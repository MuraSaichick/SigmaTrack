using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Users.SearchUsers
{
    public record UserSearchResultDto(
      Guid Id,
      string FullName,
      string Email,
      string? AvatarUrl,
      string? Position
  );
}
