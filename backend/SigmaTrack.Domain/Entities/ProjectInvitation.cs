using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Entities
{
    public class ProjectInvitation
    {
        public Guid Id { get; set; }

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public Guid InviteeId { get; set; }
        public User Invitee { get; set; } = null!;

        public Guid InviterId { get; set; }
        public User Inviter { get; set; } = null!;

        public int ProjectRoleId { get; set; }
        public ProjectRole ProjectRole { get; set; } = null!;

        public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
        public DateTime InvitedAt { get; set; }
        public DateTime? RespondedAt { get; set; } 
    }
}

