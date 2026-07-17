using SigmaTrack.Domain.Entities.ValueObjects;
using SigmaTrack.Domain.Enums;
using SigmaTrack.Domain.Exceptions;

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
        public PrivacySettings Privacy { get; set; } = new();
        public DateTime CreatedAt { get; set; }

        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<IssueHistory> AuditLogs { get; set; } = new List<IssueHistory>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<ProjectInvitation> IncomingInvitations { get; set; } = new List<ProjectInvitation>();
        public ICollection<ProjectInvitation> OutgoingInvitations { get; set; } = new List<ProjectInvitation>();
        public void ChangeEmail(string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
            {
                throw new DomainException("Email не может быть пустым.");
            }
            var formattedEmail = newEmail.Trim().ToLowerInvariant();

            if (string.Equals(Email, formattedEmail, StringComparison.Ordinal))
            {
                throw new DomainException("Новый Email совпадает с текущим.");
            }
            Email = formattedEmail;
        }
        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
            {
                throw new DomainException("Новый пароль не может быть пустым.");
            }
            if (string.Equals(HashPassword, newPasswordHash, StringComparison.Ordinal))
            {
                throw new DomainException("Новый пароль не должен совпадать с текущим паролем.");
            }
            HashPassword = newPasswordHash;
        }
        public void UpdatePrivacy(PrivacySettings newPrivacy)
        {
            ArgumentNullException.ThrowIfNull(newPrivacy);
            ValidatePrivacySettings(newPrivacy);
            Privacy = new PrivacySettings
            {
                ShowContacts = newPrivacy.ShowContacts,
                ShowBirthDate = newPrivacy.ShowBirthDate,
                ShowOnlineStatus = newPrivacy.ShowOnlineStatus,
                WhoCanInviteMe = newPrivacy.WhoCanInviteMe,
                Searchable = newPrivacy.Searchable,
                ShowStatusMessage = newPrivacy.ShowStatusMessage
            };
        }
        private void ValidatePrivacySettings(PrivacySettings settings)
        {
            if (!Enum.IsDefined(typeof(ContactVisibility), settings.ShowContacts))
            {
                throw new ArgumentException("Некорректное значение видимости контактов.", nameof(settings.ShowContacts));
            }
            if (!Enum.IsDefined(typeof(BirthDateVisibility), settings.ShowBirthDate))
            {
                throw new ArgumentException("Некорректное значение видимости даты рождения.", nameof(settings.ShowBirthDate));
            }

            if (!Enum.IsDefined(typeof(InvitationRestriction), settings.WhoCanInviteMe))
            {
                throw new ArgumentException("Некорректное ограничение на приглашения.", nameof(settings.WhoCanInviteMe));
            }
            if (!settings.Searchable && settings.WhoCanInviteMe == InvitationRestriction.Everyone)
            {
                settings.WhoCanInviteMe = InvitationRestriction.ProjectMembersOnly;
            }
        }
    }
}