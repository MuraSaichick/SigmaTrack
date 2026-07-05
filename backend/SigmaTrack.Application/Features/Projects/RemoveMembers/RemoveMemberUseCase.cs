using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.RemoveMembers
{
    public class RemoveMemberUseCase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RemoveMemberUseCase(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task ExecuteAsync(Guid projectId, Guid userId)
        {
            var project = await _projectRepository.GetWithMembersByIdAsync(projectId);
            if (project == null) throw new Exception("Проект не найден");
            project.RemoveMember(userId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}