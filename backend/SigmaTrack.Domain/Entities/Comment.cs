using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; private set; }
        public string Text { get; private set; } = null!;
        public Guid AuthorId { get; private set; }
        public User Author { get; private set; } = null!;
        public bool IsInternal { get; private set; }
        public List<string> Mentions { get; private set; } = new();
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Guid? IssueId { get; private set; }
        public Guid? ProjectId { get; private set; }
        public Guid? UserProfileId { get; private set; }

        private readonly List<Attachment> _attachments = new();
        public IReadOnlyCollection<Attachment> Attachments => _attachments.AsReadOnly();
        private Comment() { }
        public static Comment CreateForIssue(Guid issueId, Guid authorId, string text, bool isInternal = false, List<string>? mentions = null)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Текст комментария не может быть пустым.");

            return new Comment
            {
                Id = Guid.NewGuid(),
                IssueId = issueId,
                AuthorId = authorId,
                Text = text.Trim(),
                IsInternal = isInternal,
                Mentions = mentions ?? new List<string>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
        public void AddAttachments(List<(string Filename, string FileUrl, long FileSize, string ContentType)> attachmentsData)
        {
            if (attachmentsData == null) return;

            foreach (var data in attachmentsData)
            {
                var attachment = Attachment.Create(
                    data.Filename,
                    data.FileUrl,
                    data.FileSize,
                    data.ContentType
                );
                attachment.SetIds(this.Id, this.IssueId.Value);

                _attachments.Add(attachment);
            }

            UpdatedAt = DateTime.UtcNow;
        }
    }
}
