using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities.ValueObjects;
using SigmaTrack.Domain.Enums;
using System.Text.Json.Serialization;

namespace SigmaTrack.Application.Features.UserSettings.GetPrivacy;

public record PrivacySettingsResponse(
    string ShowContacts,
    string ShowBirthDate,
    bool ShowOnlineStatus,
    string WhoCanInviteMe,
    bool Searchable,
    bool ShowStatusMessage
);

public interface IGetPrivacySettingsUseCase
{
    Task<PrivacySettingsResponse> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default);
}

public class GetPrivacySettingsUseCase : IGetPrivacySettingsUseCase
{
    private readonly IUserRepository _userRepository;
    public GetPrivacySettingsUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PrivacySettingsResponse> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException("Пользователь не найден.");
        }

        var privacy = user.Privacy;

        return new PrivacySettingsResponse(
            privacy.ShowContacts.ToString(),
            privacy.ShowBirthDate.ToString(),
            privacy.ShowOnlineStatus,
            privacy.WhoCanInviteMe.ToString(),
            privacy.Searchable,
            privacy.ShowStatusMessage
        );
    }
}