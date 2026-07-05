using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Entities
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public Guid IssueId { get; set; }
        public Issue Issue { get; set; } = null!;
        public string Filename { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
    }
}
