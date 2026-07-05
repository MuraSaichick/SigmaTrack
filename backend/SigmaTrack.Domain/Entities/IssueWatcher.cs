using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Entities
{
    public class IssueWatcher
    {
        public Guid Id { get; set; }
        public Guid IssueId { get; set; }
        public Issue Issue { get; set; } = null!;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime WatchedSince { get; set; }
    }
}
