using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.CreateInvitation
{
    public record CreateInvitationResponse(Guid InvitationId, string Status);
}
