using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Entities
{
    public class IssueComment
    {
        public Guid Id { get; set; }
        public Guid IssueId { get; set; }
        public Issue Issue { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsInternal { get; set; }
        public List<string> Mentions { get; set; } = new();
        public ICollection<CommentAttachment> Attachments { get; set; } = new List<CommentAttachment>();

    }

}