using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml.Linq;

namespace SigmaTrack.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = null!;
        public int RoleId { get; set; }
        public GlobalRole Role { get; set; } = null!;
        public string HashPassword { get; set; } = null!;
        public string Firstname { get; set; } = null!;
        public string? Patronymic { get; set; }
        public string Lastname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string? AvatarColor { get; set; }
        public string? StatusMessage { get; set; }
        public UserOnlineStatus OnlineStatus { get; set; } = UserOnlineStatus.Offline;
        public string? Bio { get; set; }
        public string? Position { get; set; }
        public string? Department { get; set; }
        public List<string> Skills { get; set; } = new();
        public DateTime? BirthDate { get; set; }
        public string? Telegram { get; set; }
        public string? GitHub { get; set; }
        public DateTime? LastSeenAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<IssueHistory> AuditLogs { get; set; } = new List<IssueHistory>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<ProjectInvitation> IncomingInvitations { get; set; } = new List<ProjectInvitation>();
        public ICollection<ProjectInvitation> OutgoingInvitations { get; set; } = new List<ProjectInvitation>();
    }
}