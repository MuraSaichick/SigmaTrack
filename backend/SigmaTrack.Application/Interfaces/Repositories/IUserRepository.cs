using SigmaTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByLoginOrEmailOrPhoneAsync(string login, string email, string phone, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsAsync(Guid userId);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        void Update(User user);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<List<User>> SearchAsync(string query, int limit, CancellationToken cancellationToken = default);

    }
}
