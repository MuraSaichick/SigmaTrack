using SigmaTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Projects
{
    public interface IProjectAccessValidator
    {
        Task ValidateMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
    }
    public class ProjectAccessValidator : IProjectAccessValidator
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectAccessValidator(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task ValidateMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
        {
            bool hasAccess = await _projectRepository.IsProjectMemberAsync(projectId, userId);
            if (!hasAccess)
            {
                throw new ApplicationException("Доступ к операции запрещен. Вы не являетесь участником проекта.");
            }
        }
    }
}
