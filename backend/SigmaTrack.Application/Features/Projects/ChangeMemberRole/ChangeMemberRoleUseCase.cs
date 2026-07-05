using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.ChangeMemberRole
{
    public interface IChangeMemberRoleUseCase
    {
        Task ExecuteAsync(ChangeMemberRoleCommand command, CancellationToken cancellationToken);
    }
    public class ChangeMemberRoleUseCase : IChangeMemberRoleUseCase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeMemberRoleUseCase(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task ExecuteAsync(ChangeMemberRoleCommand command, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetWithMembersByIdAsync(command.ProjectId, cancellationToken);
            if (project == null) throw new KeyNotFoundException("Проект не найден.");
            project.ChangeMemberRole(command.UserId, command.NewRole);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
