using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Entities
{
    public class IssueLink
    {
        public Guid Id { get; set; }
        public Guid SourceIssueId { get; set; }
        public Issue SourceIssue { get; set; } = null!;
        public Guid TargetIssueId { get; set; }
        public Issue TargetIssue { get; set; } = null!;
        public IssueLinkType LinkType { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public User CreatedByUser { get; set; } = null!;
    }
}
