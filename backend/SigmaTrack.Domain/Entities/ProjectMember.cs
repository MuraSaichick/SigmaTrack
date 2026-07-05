using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Entities
{
    public class ProjectMember
    {
        public ProjectMember() { }
        public ProjectMember(Guid projectId, Guid userId, int projectRoleId)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("Project ID не может быть пустым.", nameof(projectId));

            if (userId == Guid.Empty)
                throw new ArgumentException("User ID не может быть пустым.", nameof(userId));

            if (projectRoleId <= 0)
                throw new ArgumentException("Некорректный ID роли проекта.", nameof(projectRoleId));

            Id = Guid.NewGuid();
            ProjectId = projectId;
            UserId = userId;
            ProjectRoleId = projectRoleId;
            JoinedAt = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public int ProjectRoleId { get; set; }
        public ProjectRole ProjectRole { get; set; } = null!;
        public DateTime JoinedAt { get; set; }
        internal void UpdateRole(ProjectRoleEnum newRole)
        {
            ProjectRoleId = (int)newRole;
        }
    }
}
