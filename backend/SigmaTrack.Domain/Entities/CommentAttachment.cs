using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Entities
{
    public class CommentAttachment
    {
        public Guid Id { get; set; }
        public Guid CommentId { get; set; }
        public IssueComment Comment { get; set; } = null!;
        public string Filename { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
    }
}
