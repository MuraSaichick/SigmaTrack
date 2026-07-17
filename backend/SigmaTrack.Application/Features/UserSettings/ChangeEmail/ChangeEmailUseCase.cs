using FluentValidation;
using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Exceptions;

namespace SigmaTrack.Application.Features.UserSettings.ChangeEmail;
public record ChangeEmailRequest(string NewEmail);

public record ChangeEmailResponse(bool IsSuccess, string Message);

public class ChangeEmailValidator : AbstractValidator<ChangeEmailRequest>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("Email обязателен для заполнения.")
            .EmailAddress().WithMessage("Некорректный формат Email.");
    }
}

public interface IChangeEmailUseCase
{
    Task<ChangeEmailResponse> ExecuteAsync(Guid userId, ChangeEmailRequest request, CancellationToken cancellationToken = default);
}

public class ChangeEmailUseCase : IChangeEmailUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ChangeEmailRequest> _validator;

    public ChangeEmailUseCase(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IValidator<ChangeEmailRequest> validator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ChangeEmailResponse> ExecuteAsync(
        Guid userId,
        ChangeEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var formattedEmail = request.NewEmail.Trim().ToLowerInvariant();
        bool isEmailTaken = await _userRepository.ExistsByEmailAsync(formattedEmail, cancellationToken);
        if (isEmailTaken)
        {
            throw new DomainException("Этот Email уже занят другим пользователем.");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException("Пользователь не найден.");
        }
        user.ChangeEmail(formattedEmail);
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChangeEmailResponse(true, "Email успешно изменен.");
    }
}