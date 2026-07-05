namespace SigmaTrack.Application.Features.Projects.GetProjectMembers
{
    public record ProjectMemberDto(
        Guid MemberId,
        Guid UserId,
        string FirstName,
        string Lastname,
        string? Patronymic,
        string Email,
        int ProjectRoleId,
        string ProjectRoleName,
        DateTime JoinedAt
    );
}
