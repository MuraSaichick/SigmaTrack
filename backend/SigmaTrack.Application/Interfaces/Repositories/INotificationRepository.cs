using SigmaTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
        Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, bool? isRead, CancellationToken cancellationToken);
        void Update(Notification notification, CancellationToken cancellationToken = default);
    }
}
