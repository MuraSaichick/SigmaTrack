using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.RemoveMember
{
    public record RemoveMemberCommand(Guid ProjectId, Guid UserId);
}
