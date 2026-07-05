using Microsoft.EntityFrameworkCore;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;
using SigmaTrack.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Infrastructure.Repositories
{
    public class ProjectInvitationRepository : IProjectInvitationRepository
    {
        private readonly AppDbContext _context;

        public ProjectInvitationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectInvitation?> GetByIdAsync(Guid id)
        {
            return await _context.ProjectInvitations
                .Include(pi => pi.Project)
                .Include(pi => pi.Invitee)
                .FirstOrDefaultAsync(pi => pi.Id == id);
        }
        public async Task<IEnumerable<ProjectInvitation>> GetPendingInvitationsByUserIdAsync(Guid userId)
        {
            return await _context.ProjectInvitations
                .Include(pi => pi.Project)
                .Include(pi => pi.Inviter)
                .Include(pi => pi.ProjectRole)
                .Where(pi => pi.InviteeId == userId && pi.Status == InvitationStatus.Pending)
                .OrderByDescending(pi => pi.InvitedAt)
                .ToListAsync();
        }
        public async Task AddAsync(ProjectInvitation invitation)
        {
            await _context.ProjectInvitations.AddAsync(invitation);
        }

        public async Task UpdateAsync(ProjectInvitation invitation)
        {
            _context.ProjectInvitations.Update(invitation);
            await Task.CompletedTask;
        }
        public async Task<bool> HasActiveInvitationAsync(Guid projectId, Guid inviteeId)
        {
            return await _context.ProjectInvitations
                .AnyAsync(pi => pi.ProjectId == projectId
                             && pi.InviteeId == inviteeId
                             && pi.Status == InvitationStatus.Pending);
        }
    }
}
