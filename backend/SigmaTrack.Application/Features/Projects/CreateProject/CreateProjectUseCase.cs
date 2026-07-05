using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects.CreateProject
{
    public interface ICreateProjectUseCase
    {
        Task<CreateProjectResponse> ExecuteAsync(CreateProjectCommand command, CancellationToken cancellationToken);
    }

    public class CreateProjectUseCase : ICreateProjectUseCase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProjectUseCase(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateProjectResponse> ExecuteAsync(CreateProjectCommand command, CancellationToken cancellationToken)
        {
            var upperPrefix = command.Prefix.Trim().ToUpper();
            var existingProject = await _projectRepository.GetByPrefixAsync(upperPrefix, cancellationToken);
            if (existingProject != null)
            {
                throw new ArgumentException($"Проект с префиксом '{upperPrefix}' уже существует.");
            }

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Description = command.Description,
                Prefix = upperPrefix,
                CreatorId = command.CreatorId,
                CreatedAt = DateTime.UtcNow
            };
            var managerMember = new ProjectMember(project.Id, command.CreatorId, (int)ProjectRoleEnum.ProjectManager);
            project.ProjectMembers.Add(managerMember);
            await _projectRepository.AddAsync(project, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateProjectResponse(
                project.Id,
                project.Name,
                project.Description,
                project.Prefix,
                project.CreatorId,
                project.CreatedAt
            );
        }
    }
}
