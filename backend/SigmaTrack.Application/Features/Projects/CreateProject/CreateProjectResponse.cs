using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.CreateProject
{
    public record CreateProjectResponse(
        Guid ProjectId,
        string Name,
        string? Description,
        string Prefix,
        Guid CreatorId,
        DateTime CreatedAt
        );
}