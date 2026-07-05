using Microsoft.EntityFrameworkCore;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<User?> GetByLoginOrEmailOrPhoneAsync(string login, string email,string phone, CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login || u.Email == email || u.Phone == phone);
        }
        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        public void Update(User user)
        {
            _context.Users.Update(user);
        }
        public async Task<bool> ExistsAsync(Guid userId)
        {
            return await _context.Users
                .AnyAsync(u => u.Id == userId);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<User>> SearchAsync(string query, int limit, CancellationToken cancellationToken = default)
        {
            var lowerQuery = query.ToLower();

            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Login.ToLower().Contains(lowerQuery) ||
                            u.Email.ToLower().Contains(lowerQuery) ||
                            u.Firstname.ToLower().Contains(lowerQuery) ||
                            u.Lastname.ToLower().Contains(lowerQuery))
                .Take(limit)
                .ToListAsync(cancellationToken);
        }
    }
}
