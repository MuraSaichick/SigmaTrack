using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Profile.UpdateProfile
{
    public interface IUpdateProfileUseCase
    {
        Task<UserProfileResponse> ExecuteAsync(UpdateProfileCommand command, CancellationToken cancellationToken = default);
    }
    public class UpdateProfileUseCase : IUpdateProfileUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProfileUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<UserProfileResponse> ExecuteAsync(UpdateProfileCommand command, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {command.UserId} not found.");
            }
            if (user.Id != command.UserId)
            {
                throw new UnauthorizedAccessException($"User is not authorized to perform actions on behalf of User ID {command.UserId}.");
            }
            if (string.IsNullOrWhiteSpace(command.Firstname))
                throw new ArgumentException("Имя не может быть пустым.");
            if (string.IsNullOrWhiteSpace(command.Lastname))
                throw new ArgumentException("Фамилия не может быть пустой.");
            if (string.IsNullOrWhiteSpace(command.Phone))
                throw new ArgumentException("Телефон не может быть пустым.");
            user.Firstname = command.Firstname.Trim();
            user.Lastname = command.Lastname.Trim();
            user.Phone = command.Phone.Trim();

            user.Patronymic = string.IsNullOrWhiteSpace(command.Patronymic) ? null : command.Patronymic.Trim();
            user.StatusMessage = string.IsNullOrWhiteSpace(command.StatusMessage) ? null : command.StatusMessage.Trim();
            user.Bio = string.IsNullOrWhiteSpace(command.Bio) ? null : command.Bio.Trim();
            user.Position = string.IsNullOrWhiteSpace(command.Position) ? null : command.Position.Trim();
            user.Department = string.IsNullOrWhiteSpace(command.Department) ? null : command.Department.Trim();
            user.Telegram = string.IsNullOrWhiteSpace(command.Telegram) ? null : command.Telegram.Trim();
            user.GitHub = string.IsNullOrWhiteSpace(command.GitHub) ? null : command.GitHub.Trim();
            user.BirthDate = command.BirthDate;
            user.Skills = command.Skills != null
             ? command.Skills.Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList()
             : new List<string>();
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UserProfileResponse(
            user.Id,
            user.Login,
            user.Firstname,
            user.Lastname,
            user.Patronymic,
            user.Email,
            user.Phone,
            user.AvatarUrl,
            user.AvatarColor,
            user.StatusMessage,
            user.Bio,
            user.Position,
            user.Department,
            user.Skills,
            user.BirthDate,
            user.Telegram,
            user.GitHub
        );
        }
    }
}
