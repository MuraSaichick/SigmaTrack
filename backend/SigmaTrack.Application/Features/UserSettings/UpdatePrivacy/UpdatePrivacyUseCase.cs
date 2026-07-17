using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities.ValueObjects;
using SigmaTrack.Domain.Enums;


namespace SigmaTrack.Application.Features.UserSettings.UpdatePrivacy
{
    public record UpdatePrivacyRequest(
        string ShowContacts,
        string ShowBirthDate,
        bool ShowOnlineStatus,
        string WhoCanInviteMe,
        bool Searchable,
        bool ShowStatusMessage
    );

    public record UpdatePrivacyResponse(bool IsSuccess, string Message);
    public interface IUpdatePrivacyUseCase
    {
        Task<UpdatePrivacyResponse> ExecuteAsync(Guid userId, UpdatePrivacyRequest request, CancellationToken cancellationToken = default);
    }
    public class UpdatePrivacyUseCase : IUpdatePrivacyUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePrivacyUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdatePrivacyResponse> ExecuteAsync(
            Guid userId,
            UpdatePrivacyRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return new UpdatePrivacyResponse(false, "Пользователь не найден");
            }
            if (!Enum.TryParse<ContactVisibility>(request.ShowContacts, true, out var showContactsEnum))
            {
                throw new ArgumentException($"Некорректное значение для видимости контактов: {request.ShowContacts}");
            }

            if (!Enum.TryParse<BirthDateVisibility>(request.ShowBirthDate, true, out var showBirthDateEnum))
            {
                throw new ArgumentException($"Некорректное значение для видимости даты рождения: {request.ShowBirthDate}");
            }

            if (!Enum.TryParse<InvitationRestriction>(request.WhoCanInviteMe, true, out var whoCanInviteMeEnum))
            {
                throw new ArgumentException($"Некорректное значение для ограничений приглашений: {request.WhoCanInviteMe}");
            }
            var newPrivacy = new PrivacySettings
            {
                ShowContacts = showContactsEnum,
                ShowBirthDate = showBirthDateEnum,
                ShowOnlineStatus = request.ShowOnlineStatus,
                WhoCanInviteMe = whoCanInviteMeEnum,
                Searchable = request.Searchable,
                ShowStatusMessage = request.ShowStatusMessage
            };
            user.UpdatePrivacy(newPrivacy);

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdatePrivacyResponse(true, "Настройки приватности успешно обновлены");
        }
    }
}
