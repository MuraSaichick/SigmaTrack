using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.AcceptInvitation
{
    public record AcceptInvitationCommand(Guid InvitationId, Guid UserId);
}
