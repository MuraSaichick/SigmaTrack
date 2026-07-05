using FluentValidation;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Features.Issues.GetIssuesList
{
    public record IssueDto(
    Guid Id,
    int Number,
    string Title,
    int Status,
    int Type,
    int Priority,
    int? StoryPoints,
    Guid? AssigneeId,
    DateTime UpdatedAt
);

    public record PagedListResponse<T>(
        System.Collections.Generic.IReadOnlyCollection<T> Items,
        int TotalCount,
        int PageNumber,
        int PageSize
    );
    public record GetIssuesListQuery(
    Guid ProjectId,
    IssueStatus? Status,
    IssueType? Type,
    IssuePriority? Priority,
    Guid? AssigneeId,
    string? SearchTerm,
    int PageNumber = 1,
    int PageSize = 20
        );
    public interface IGetIssuesListUseCase
    {
        Task<PagedListResponse<IssueDto>> ExecuteAsync(GetIssuesListQuery query, CancellationToken cancellationToken);
    }
    public class GetIssuesListUseCase : IGetIssuesListUseCase
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IValidator<GetIssuesListQuery> _validator;

        public GetIssuesListUseCase(IIssueRepository issueRepository, IValidator<GetIssuesListQuery> validator)
        {
            _issueRepository = issueRepository;
            _validator = validator;
        }

        public async Task<PagedListResponse<IssueDto>> ExecuteAsync(GetIssuesListQuery query, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(query, cancellationToken);
            var (items, totalCount) = await _issueRepository.GetListAsync(query, cancellationToken);

            return new PagedListResponse<IssueDto>(items, totalCount, query.PageNumber, query.PageSize);
        }
    }
    public class GetIssuesListQueryValidator : AbstractValidator<GetIssuesListQuery>
    {
        public GetIssuesListQueryValidator()
        {
            RuleFor(x => x.ProjectId).NotEmpty().WithMessage("Идентификатор проекта обязателен.");
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1).WithMessage("Номер страницы должен быть не меньше 1.");
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("Размер страницы должен быть от 1 до 100 элементов.");
        }
    }
}
