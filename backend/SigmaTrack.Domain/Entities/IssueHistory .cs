using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Entities
{
    public class IssueHistory
    {
        public Guid Id { get; set; }
        public Guid IssueId { get; set; }
        public Issue Issue { get; set; } = null!;
        public Guid ChangedBy { get; set; }
        public User User { get; set; } = null!;
        public string FieldName { get; set; } = null!; 
        public string? OldValue { get; set; }            
        public string? NewValue { get; set; }            
        public DateTime ChangedAt { get; set; }
    }
}
