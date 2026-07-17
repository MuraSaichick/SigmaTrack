using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Enums;

namespace SigmaTrack.Application.Features.Users.GetUserProfile;

public record UserProfileResponse(
    Guid Id,
    string Login,
    string Firstname,
    string? Patronymic,
    string Lastname,
    string? AvatarUrl,
    string? AvatarColor,
    string? Bio,
    string? Position,
    string? Department,
    List<string> Skills,
    string? GitHub,
    DateTime? LastSeenAt,
    UserOnlineStatus OnlineStatus,

    string? Email,
    string? PhoneNumber,
    string? Telegram,
    DateTime? BirthDate,

    bool ShowStatusMessage,
    string? StatusMessage
);

public interface IGetUserProfileUseCase
{
    Task<UserProfileResponse> ExecuteAsync(Guid currentUserId, Guid targetUserId, CancellationToken cancellationToken = default);
}

public class GetUserProfileUseCase : IGetUserProfileUseCase
{
    private readonly IUserRepository _userRepository;

    public GetUserProfileUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileResponse> ExecuteAsync(Guid currentUserId, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        var targetUser = await _userRepository.GetByIdAsync(targetUserId, cancellationToken);
        if (targetUser == null)
        {
            throw new KeyNotFoundException("Пользователь не найден.");
        }
        if (currentUserId == targetUserId)
        {
            return MapFullProfile(targetUser);
        }

        bool areInSameTeam = await _userRepository.AreUsersInSameTeamAsync(currentUserId, targetUserId, cancellationToken);

        string? email = null;
        string? phoneNumber = null;
        string? telegram = null;
        DateTime? birthDate = null;

        if (targetUser.Privacy.ShowContacts == ContactVisibility.Everyone ||
           (targetUser.Privacy.ShowContacts == ContactVisibility.TeamOnly && areInSameTeam))
        {
            email = targetUser.Email;
            phoneNumber = targetUser.Phone;
            telegram = targetUser.Telegram;
        }

        if (targetUser.Privacy.ShowBirthDate == BirthDateVisibility.FullDate ||
            targetUser.Privacy.ShowBirthDate == BirthDateVisibility.MonthAndDayOnly)
        {
            birthDate = targetUser.BirthDate;
        }

        var onlineStatus = targetUser.Privacy.ShowOnlineStatus
            ? targetUser.OnlineStatus
            : UserOnlineStatus.Offline;

        return new UserProfileResponse(
            targetUser.Id,
            targetUser.Login,
            targetUser.Firstname,
            targetUser.Patronymic,
            targetUser.Lastname,
            targetUser.AvatarUrl,
            targetUser.AvatarColor,
            targetUser.Bio,
            targetUser.Position,
            targetUser.Department,
            targetUser.Skills,
            targetUser.GitHub,
            targetUser.LastSeenAt,
            onlineStatus,
            email,
            phoneNumber,
            telegram,
            birthDate,
            targetUser.Privacy.ShowStatusMessage,
            targetUser.Privacy.ShowStatusMessage ? targetUser.StatusMessage : null
        );
    }

    private UserProfileResponse MapFullProfile(Domain.Entities.User user)
    {
        return new UserProfileResponse(
            user.Id,
            user.Login,
            user.Firstname,
            user.Patronymic,
            user.Lastname,
            user.AvatarUrl,
            user.AvatarColor,
            user.Bio,
            user.Position,
            user.Department,
            user.Skills,
            user.GitHub,
            user.LastSeenAt,
            user.OnlineStatus,
            user.Email,
            user.Phone,
            user.Telegram,
            user.BirthDate,
            user.Privacy.ShowStatusMessage,
            user.StatusMessage
        );
    }
}