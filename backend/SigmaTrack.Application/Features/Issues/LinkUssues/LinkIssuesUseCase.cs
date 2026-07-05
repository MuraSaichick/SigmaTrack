using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;

namespace SigmaTrack.Application.Features.Issues.LinkIssues;
public record LinkIssuesCommand(
    Guid SourceIssueId,
    Guid TargetIssueId,
    IssueLinkType LinkType,
    string? Description,
    Guid UserId);

public record LinkIssuesResponse(
    Guid LinkId,
    Guid SourceIssueId,
    Guid TargetIssueId,
    IssueLinkType LinkType);
public interface ILinkIssuesUseCase
{
    Task<LinkIssuesResponse> ExecuteAsync(LinkIssuesCommand command, CancellationToken cancellationToken = default);
}
public class LinkIssuesUseCase : ILinkIssuesUseCase
{
    private readonly Interfaces.Repositories.IIssueRepository _issueRepository;
    private readonly Interfaces.IUnitOfWork _unitOfWork;
    private readonly IValidator<LinkIssuesCommand> _validator;

    public LinkIssuesUseCase(
        Interfaces.Repositories.IIssueRepository issueRepository,
        Interfaces.IUnitOfWork unitOfWork,
        IValidator<LinkIssuesCommand> validator)
    {
        _issueRepository = issueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<LinkIssuesResponse> ExecuteAsync(LinkIssuesCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        if (command.SourceIssueId == command.TargetIssueId)
        {
            throw new ArgumentException("Нельзя связать задачу саму с собой.");
        }
        var sourceIssue = await _issueRepository.GetByIdAsync(command.SourceIssueId, cancellationToken);
        if (sourceIssue == null)
        {
            throw new KeyNotFoundException($"Исходная задача с ID {command.SourceIssueId} не найдена.");
        }
        var targetIssue = await _issueRepository.GetByIdAsync(command.TargetIssueId, cancellationToken);
        if (targetIssue == null)
        {
            throw new KeyNotFoundException($"Целевая задача с ID {command.TargetIssueId} не найдена.");
        }
        IssueLink createdLink = sourceIssue.AddLink(
            command.TargetIssueId,
            command.LinkType,
            command.Description,
            command.UserId
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LinkIssuesResponse(
            createdLink.Id,
            createdLink.SourceIssueId,
            createdLink.TargetIssueId,
            createdLink.LinkType
        );
    }
}
public class LinkIssuesCommandValidator : AbstractValidator<LinkIssuesCommand>
{
    public LinkIssuesCommandValidator()
    {
        RuleFor(x => x.SourceIssueId).NotEmpty().WithMessage("Идентификатор исходной задачи обязателен.");
        RuleFor(x => x.TargetIssueId).NotEmpty().WithMessage("Идентификатор целевой задачи обязателен.");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Идентификатор пользователя обязателен.");
    }
}