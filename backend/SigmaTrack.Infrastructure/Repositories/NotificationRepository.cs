using Microsoft.EntityFrameworkCore;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Infrastructure.Data;

namespace SigmaTrack.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;
        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _context.Notifications.AddAsync(notification);
        }
        public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, bool? isRead, CancellationToken cancellationToken)
        {
            var result = _context.Notifications.Where(n => n.UserId == userId);

            if(isRead.HasValue)
            {
                result = result.Where(r => r.IsRead == isRead);            }
            return await result
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public void Update(Notification notification, CancellationToken cancellationToken = default)
        {
            _context.Notifications.Update(notification);
        }
    }
}