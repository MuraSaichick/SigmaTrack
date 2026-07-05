using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.RemoveMember
{
    public interface IRemoveMemberUseCase
    {
        Task ExecuteAsync(RemoveMemberCommand command, CancellationToken cancellationToken);
    }

    public class RemoveMemberUseCase : IRemoveMemberUseCase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveMemberUseCase(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(RemoveMemberCommand command, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetWithMembersByIdAsync(command.ProjectId, cancellationToken);
            if (project == null) throw new KeyNotFoundException("Проект не найден.");
            project.RemoveMember(command.UserId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
