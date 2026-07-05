using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Enums
{
    public enum IssueStatus
    {
        Backlog = 0,
        ToDo = 1,
        InProgress = 2,
        InReview = 3,
        Testing = 4,
        Resolved = 5,
        Closed = 6,
        Reopened = 7,
        Rejected = 8,
        OnHold = 9
    }
}