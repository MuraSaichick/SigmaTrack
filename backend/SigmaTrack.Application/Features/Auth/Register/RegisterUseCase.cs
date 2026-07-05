using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;
using SigmaTrack.Domain.Exceptions;

namespace SigmaTrack.Application.Features.Auth.Register;

public interface IRegisterUseCase
{
    Task ExecuteAsync(RegisterRequest request, CancellationToken cancellationToken);
}

public class RegisterUseCase : IRegisterUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByLoginOrEmailOrPhoneAsync(
            request.Login, request.Email, request.Phone, cancellationToken);

        if (existingUser != null)
        {
            throw new DomainException("Пользователь с таким логином, email или телефоном уже существует!");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = request.Login,
            Email = request.Email,
            Phone = request.Phone,
            HashPassword = passwordHash,
            RoleId = (int)GlobalRoleEnum.User,
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Patronymic = request.Patronymic,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

