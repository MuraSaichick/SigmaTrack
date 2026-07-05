using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.DeleteProject
{
    public interface IDeleteProjectUseCase
    {
        Task ExecuteAsync(DeleteProjectCommand command, CancellationToken cancellationToken);
    }

    public class DeleteProjectUseCase : IDeleteProjectUseCase
    {
        private readonly IProjectRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProjectUseCase(IProjectRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(DeleteProjectCommand command, CancellationToken cancellationToken)
        {
            bool isMember = await _repository.IsProjectMemberAsync(command.ProjectId, command.UserId);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("Вы не являетесь участником этого проекта.");
            }
            var project = await _repository.GetByIdAsync(command.ProjectId, cancellationToken);
            if (project == null)
            {
                throw new KeyNotFoundException("Проект не найден.");
            }
            if (!project.CanBeDeletedBy(command.UserId))
            {
                throw new InvalidOperationException("Только создатель проекта может его удалить.");
            }
            await _repository.DeleteAsync(project, cancellationToken);
            _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}