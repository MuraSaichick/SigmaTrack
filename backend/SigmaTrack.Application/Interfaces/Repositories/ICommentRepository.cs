using SigmaTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Interfaces.Repositories
{
   public interface ICommentRepository
    {
        Task AddAsync(Comment comment, CancellationToken cancellationToken);
    }
}
