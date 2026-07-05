using System;
using System.Collections.Generic;
using System.Text;
using SigmaTrack.Domain.Enums;

namespace SigmaTrack.Application.Features.Projects.GetUserProjects
{
    public record UserProjectDto(Guid ProjectId, string Name, string? Description, string Prefix, Guid CreatorId, DateTime CreatedAt, int ProjectRoleId);
}