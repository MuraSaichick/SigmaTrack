using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Auth.Login
{
    public interface ILoginUseCase
    {
        Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken);
    }

    public class LoginUseCase : ILoginUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;

        public LoginUseCase(IUserRepository userRepository, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
        }

        public async Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByLoginOrEmailOrPhoneAsync(
                request.Login, request.Login, request.Login, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Неверный логин или пароль.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.HashPassword);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Неверный логин или пароль.");
            }
            string token = _jwtProvider.GenerateToken(user);
            return new LoginResponse(user.Id, token, user.Login, user.Email, user.Firstname, user.Lastname, user.AvatarUrl);
        }
    }
}
