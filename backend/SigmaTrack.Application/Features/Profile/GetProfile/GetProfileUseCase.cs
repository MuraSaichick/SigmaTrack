using SigmaTrack.Application.Features.Profile.UpdateProfile;
using SigmaTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Profile.GetProfile
{
    public interface IGetProfileUseCase
    {
        Task<UserProfileResponse> ExecuteAsync(Guid userId, CancellationToken cancellationToken);
    }
    public class GetProfileUseCase : IGetProfileUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetProfileUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserProfileResponse> ExecuteAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException($"Пользователь с ID {userId} не найден.");
            }

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
