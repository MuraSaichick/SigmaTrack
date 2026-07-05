using SigmaTrack.Domain.Enums;

namespace SigmaTrack.Application.Features.Projects.ChangeMemberRole
{
    public record ChangeMemberRoleRequest(ProjectRoleEnum NewRole);
    public record ChangeMemberRoleCommand(Guid ProjectId, Guid UserId, ProjectRoleEnum NewRole);
}
