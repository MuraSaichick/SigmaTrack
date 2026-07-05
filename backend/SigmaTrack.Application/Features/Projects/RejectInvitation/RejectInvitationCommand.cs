using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.RejectInvitation
{
    public record RejectInvitationCommand(Guid InvitationId, Guid UserId);
}
