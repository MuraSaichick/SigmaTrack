
using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Exceptions;

namespace SigmaTrack.Application.Features.UserSettings.ChangePassword;
public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
);

public record ChangePasswordResponse(bool IsSuccess, string? ErrorMessage = null);
public interface IChangePasswordUseCase
{
    Task<ChangePasswordResponse> ExecuteAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
}
public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordUseCase(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ChangePasswordResponse> ExecuteAsync(
        Guid userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            return new ChangePasswordResponse(false, "Новые пароли не совпадают.");
        }

        if (request.NewPassword.Length < 8)
        {
            return new ChangePasswordResponse(false, "Пароль должен быть не менее 8 символов.");
        }
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return new ChangePasswordResponse(false, "Пользователь не найден.");
        }
        bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.HashPassword);
        if (!isCurrentPasswordValid)
        {
            return new ChangePasswordResponse(false, "Неверно указан текущий пароль.");
        }
        string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.ChangePassword(newPasswordHash);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChangePasswordResponse(true);
    }
}