using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.UpdateProjectDetails
{
    public record UpdateProjectDetailsRequest(string Name, string Prefix, string? Description);
    public record UpdateProjectDetailsCommand(Guid ProjectId, string Name, string Prefix, string? Description);
}
