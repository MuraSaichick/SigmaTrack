using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.UpdateProjectDetails
{
    public interface IUpdateProjectDetailsUseCase
    {
        Task ExecuteAsync(UpdateProjectDetailsCommand command, CancellationToken cancellationToken);
    }

    public class UpdateProjectDetailsUseCase : IUpdateProjectDetailsUseCase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProjectDetailsUseCase(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(UpdateProjectDetailsCommand command, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetWithMembersByIdAsync(command.ProjectId, cancellationToken);
            if (project == null) throw new KeyNotFoundException("Проект не найден.");
            project.UpdateDetails(command.Name, command.Prefix, command.Description);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
