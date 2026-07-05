using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.CreateInvitation
{
    public record CreateInvitationCommand(
        Guid ProjectId,
        Guid InviterId,
        string InviteeEmail,
        int ProjectRoleId
        );
}
