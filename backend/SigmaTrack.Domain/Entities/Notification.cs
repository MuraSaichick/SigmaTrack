using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Entities
{

    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public NotificationType Type { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        public string? LinkUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        private Notification() { }

        public static Notification CreateIssueAssigned(Guid assigneeId, Guid projectId, Guid issueId, string issueTitle)
        {
            return new Notification
            {
                Id = Guid.NewGuid(),
                UserId = assigneeId,
                Type = NotificationType.IssueAssigned,
                Title = "Новое назначение",
                Message = $"Вы назначены исполнителем по задаче \"{issueTitle}\".",
                LinkUrl = $"/projects/{projectId}/issues/{issueId}",
                CreatedAt = DateTimeOffset.UtcNow,
                IsRead = false
            };
        }
        public static Notification CreateProjectInvitationAccepted(Guid inviterId, string inviteeName, string projectName)
        {
            return new Notification
            {
                Id = Guid.NewGuid(),
                UserId = inviterId,
                Type = NotificationType.ProjectInvitationAccepted,
                Title = "Приглашение принято",
                Message = $"{inviteeName} принял ваше приглашение в проект \"{projectName}\".",
                IsRead = false,
                CreatedAt = DateTimeOffset.UtcNow
            };
        }
        public static Notification CreateProjectInvitationReceived(
            Guid inviteeId,
            ProjectRoleEnum role,
            Guid invitationId)
        {
            return new Notification
            {
                Id = Guid.NewGuid(),
                UserId = inviteeId,
                Type = NotificationType.ProjectInvitationReceived,
                Title = "Приглашение в проект",
                Message = $"Вас пригласили в проект. Роль: {role.ToString()}",
                LinkUrl = $"/api/projects/invitations/{invitationId}",
                IsRead = false,
                CreatedAt = DateTimeOffset.UtcNow
            };
        }
        public static Notification CreateProjectInvitationRejected(Guid inviterId, string inviteeName, string projectName)
        {
            return new Notification
            {
                Id = Guid.NewGuid(),
                UserId = inviterId,
                Type = NotificationType.ProjectInvitationRejected,
                Title = "Приглашение отклонено",
                Message = $"{inviteeName} отклонил(а) ваше приглашение в проект \"{projectName}\".",
                IsRead = false,
                CreatedAt = DateTimeOffset.UtcNow
            };
        }

    }
}