using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.DeleteProject
{
    public record DeleteProjectCommand(Guid ProjectId, Guid UserId);
}
