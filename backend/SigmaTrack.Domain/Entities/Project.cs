using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Prefix { get; set; } = null!;
        public Guid CreatorId { get; set; }
        public User Creator { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<Issue> Issues { get; set; } = new List<Issue>();
        public ICollection<ProjectInvitation> Invitations { get; set; } = new List<ProjectInvitation>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<Sprint> Sprints { get; set; } = new List<Sprint>();
        public bool CanBeDeletedBy(Guid userId)
        {
            return CreatorId == userId;
        }
        public void LeaveProject(Guid userId)
        {
            if (CreatorId == userId)
            {
                throw new InvalidOperationException("Создатель проекта не может покинуть его. Передайте права владельца или удалите проект.");
            }
            var member = ProjectMembers.FirstOrDefault(m => m.UserId == userId);
            if (member == null)
            {
                throw new ArgumentException("Пользователь не является участником этого проекта.");
            }
            ProjectMembers.Remove(member);
        }
        public void UpdateDetails(string name, string prefix, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название проекта не может быть пустым.");

            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("Префикс не может быть пустым.");

            Name = name;
            Prefix = prefix.ToUpper();
            Description = description;
        }
        public void RemoveMember(Guid userId)
        {
            if (userId == CreatorId)
                throw new InvalidOperationException("Нельзя удалить создателя проекта.");

            var member = ProjectMembers.FirstOrDefault(m => m.UserId == userId);
            if (member == null)
                throw new InvalidOperationException("Пользователь не является участником этого проекта.");

            ProjectMembers.Remove(member);
        }
        public void ChangeMemberRole(Guid userId, ProjectRoleEnum newRole)
        {
            var member = ProjectMembers.FirstOrDefault(m => m.UserId == userId);
            if (member == null)
                throw new InvalidOperationException("Пользователь не является участником этого проекта.");
            if (userId == CreatorId && newRole != ProjectRoleEnum.ProjectManager)
                throw new InvalidOperationException("Создатель проекта должен всегда иметь роль ProjectManager.");
            member.UpdateRole(newRole);
        }
    }
}