using Microsoft.EntityFrameworkCore;
using SigmaTrack.Application.Features.Projects.GetProjectMembers;
using SigmaTrack.Application.Features.Projects.GetUserProjects;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;
using SigmaTrack.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context) => _context = context;

        public async Task<Project?> GetByPrefixAsync(string prefix, CancellationToken cancellationToken)
        {
            return await _context.Projects
                .FirstOrDefaultAsync(p => p.Prefix == prefix, cancellationToken);
        }
        public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Projects
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<UserProjectDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Projects
                .Where(p => p.ProjectMembers.Any(pm => pm.UserId == userId))
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new UserProjectDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Prefix,
                    p.CreatorId,
                    p.CreatedAt,
                    p.ProjectMembers.First(pm => pm.UserId == userId).ProjectRoleId
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsProjectManagerOrCreatorAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Projects
                .AnyAsync(p => p.Id == projectId &&
                              (p.CreatorId == userId ||
                               p.ProjectMembers.Any(pm => pm.UserId == userId && pm.ProjectRoleId == (int)ProjectRoleEnum.ProjectManager)));
        }

        public async Task<bool> IsProjectMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
        }
        public async Task<Project?> GetWithMembersByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Projects
                .Include(p => p.ProjectMembers)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }
        public async Task AddProjectMemberAsync(ProjectMember member, CancellationToken cancellationToken = default)
        {
            await _context.ProjectMembers.AddAsync(member);
        }
        public async Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken)
        {
            return await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
        }

        public async Task AddAsync(Project project, CancellationToken cancellationToken)
        => await _context.Projects.AddAsync(project, cancellationToken);
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);

        public Task DeleteAsync(Project project, CancellationToken cancellationToken = default)
        {
            _context.Projects.Remove(project);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ProjectMemberDto>> GetMembersByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            return await _context.ProjectMembers
                .AsNoTracking()
                .Where(m => m.ProjectId == projectId)
                .Select(m => new ProjectMemberDto(
                    m.Id,
                    m.UserId,
                    m.User.Firstname,
                    m.User.Lastname,
                    m.User.Patronymic,
                    m.User.Email,
                    m.ProjectRoleId,
                    m.ProjectRole.Name,
                    m.JoinedAt
                ))
                .ToListAsync();
        }
    }
}
