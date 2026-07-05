using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Entities
{
    public class IssueTemplate
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public IssueType Type { get; set; }
        public IssuePriority Priority { get; set; }
        public string? Component { get; set; }
        public List<string> Tags { get; set; } = new();
        public bool IsDefault { get; set; }
    }
}
