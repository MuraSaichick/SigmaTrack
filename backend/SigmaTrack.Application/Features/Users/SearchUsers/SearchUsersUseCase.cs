using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;

namespace SigmaTrack.Application.Features.Users.SearchUsers
{

    public interface ISearchUsersUseCase
    {
        Task<List<UserSearchResultDto>> ExecuteAsync(string query, CancellationToken cancellationToken);
    }

    public class SearchUsersUseCase : ISearchUsersUseCase
    {
        private readonly IUserRepository _userRepository;
        public SearchUsersUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<List<UserSearchResultDto>> ExecuteAsync(string query, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<UserSearchResultDto>();
            var users = await _userRepository.SearchAsync(query, limit: 10, cancellationToken);
            return users.Select(u => new UserSearchResultDto(
                u.Id,
                $"{u.Firstname} {u.Lastname}".Trim(),
                u.Email,
                u.AvatarUrl,
                u.Position
            )).ToList();
        }
    }
}
