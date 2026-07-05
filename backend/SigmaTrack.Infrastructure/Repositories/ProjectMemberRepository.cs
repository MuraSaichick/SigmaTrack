using Microsoft.EntityFrameworkCore;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Infrastructure.Repositories
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly AppDbContext _context;

        public ProjectMemberRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<ProjectMember?> GetByIdAsync(Guid userId, Guid projectId, CancellationToken cancellationToken)
        {
            return await _context.ProjectMembers
                .Include(m => m.User)
                .Include(m => m.Project)
                .Include(m => m.ProjectRole)
                .FirstOrDefaultAsync(m => m.UserId == userId && m.ProjectId == projectId, cancellationToken);
        }

        public async Task<IEnumerable<ProjectMember>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken)
        {
            return await _context.ProjectMembers
                .Include(m => m.User)
                .Include(m => m.ProjectRole)
                .Where(m => m.ProjectId == projectId)
                .ToListAsync(cancellationToken);
        }
        public async Task<bool> IsMemberAsync(Guid userId, Guid projectId, CancellationToken cancellationToken)
        {
            return await _context.ProjectMembers
                .AnyAsync(m => m.UserId == userId && m.ProjectId == projectId, cancellationToken);
        }

        public async Task<int?> GetRoleIdAsync(Guid userId, Guid projectId, CancellationToken cancellationToken)
        {
            return await _context.ProjectMembers
                .Where(m => m.UserId == userId && m.ProjectId == projectId)
                .Select(m => (int?)m.ProjectRoleId)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task AddAsync(ProjectMember member, CancellationToken cancellationToken)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            await _context.ProjectMembers.AddAsync(member, cancellationToken);
        }

        public async Task UpdateAsync(ProjectMember member, CancellationToken cancellationToken)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            _context.ProjectMembers.Update(member);
            await Task.CompletedTask;
        }
        public async Task RemoveAsync(ProjectMember member, CancellationToken cancellationToken)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            _context.ProjectMembers.Remove(member);
            await Task.CompletedTask;
        }
    }
}
