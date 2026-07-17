using FluentValidation;
using SigmaTrack.Application.Interfaces.Repositories;

namespace SigmaTrack.Application.Features.Users.SearchUsers;

public record SearchUsersRequest(
    string Query,
    int? Page = 1,
    int? PageSize = 10
);

public record UserSearchDto(
    Guid Id,
    string Login,
    string Firstname,
    string Lastname,
    string? AvatarUrl
);

public record SearchUsersResponse(
    List<UserSearchDto> Users,
    int TotalCount
);

public class SearchUsersValidator : AbstractValidator<SearchUsersRequest>
{
    public SearchUsersValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty().WithMessage("Поисковый запрос не может быть пустым.")
            .MinimumLength(2).WithMessage("Минимальная длина запроса — 2 символа.");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Номер страницы должен быть больше 0.")
            .When(x => x.Page.HasValue);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Размер страницы должен быть от 1 до 100.")
            .When(x => x.PageSize.HasValue);
    }
}

public interface ISearchUsersUseCase
{
    Task<SearchUsersResponse> ExecuteAsync(SearchUsersRequest request, CancellationToken cancellationToken = default);
}

public class SearchUsersUseCase : ISearchUsersUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<SearchUsersRequest> _validator;

    public SearchUsersUseCase(
        IUserRepository userRepository,
        IValidator<SearchUsersRequest> validator)
    {
        _userRepository = userRepository;
        _validator = validator;
    }

    public async Task<SearchUsersResponse> ExecuteAsync(SearchUsersRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 10;
        var searchTerm = request.Query.Trim();

        var (users, totalCount) = await _userRepository.SearchActiveUsersAsync(
            searchTerm,
            page,
            pageSize,
            cancellationToken
        );

        var userDtos = users.Select(u => new UserSearchDto(
            u.Id,
            u.Login,
            u.Firstname,
            u.Lastname,
            u.AvatarUrl
        )).ToList();

        return new SearchUsersResponse(userDtos, totalCount);
    }
}