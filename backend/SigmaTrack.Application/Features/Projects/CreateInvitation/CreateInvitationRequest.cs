using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.CreateInvitation
{
    public record CreateInvitationRequest(string InviteeEmail, int ProjectRoleId);
}
