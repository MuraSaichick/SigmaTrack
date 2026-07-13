using FluentValidation;
using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Entities;

namespace SigmaTrack.Application.Features.Issues.AddComment;

public record AttachmentInput(
    string Filename,
    string FileUrl,
    long FileSize,
    string ContentType
);

public record AddCommentRequest(
    string Text,
    bool IsInternal,
    List<string>? Mentions,
    List<AttachmentInput>? Attachments
);

public record AddCommentCommand(
    Guid IssueId,
    Guid AuthorId,
    string Text,
    bool IsInternal,
    List<string> Mentions,
    List<AttachmentInput> Attachments
);

public record AttachmentResponseDto(
    Guid Id,
    string Filename,
    string FileUrl,
    long FileSize,
    string ContentType,
    DateTime UploadedAt
);

public record CommentResponseDto(
    Guid Id,
    Guid AuthorId,
    string AuthorName,
    string? AuthorAvatarUrl,
    string? AuthorAvatarColor,
    string Text,
    DateTime CreatedAt,
    bool IsInternal,
    List<AttachmentResponseDto> Attachments
);

public interface IAddCommentUseCase
{
    Task<CommentResponseDto> ExecuteAsync(AddCommentCommand command, CancellationToken cancellationToken);
}

public class AddCommentUseCase : IAddCommentUseCase
{
    private readonly IIssueRepository _issueRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AddCommentCommand> _validator;

    public AddCommentUseCase(
        IIssueRepository issueRepository,
        IUserRepository userRepository,
        ICommentRepository commentRepository,
        IUnitOfWork unitOfWork,
        IValidator<AddCommentCommand> validator)
    {
        _issueRepository = issueRepository;
        _userRepository = userRepository;
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CommentResponseDto> ExecuteAsync(AddCommentCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var issueExists = await _issueRepository.ExistsAsync(command.IssueId, cancellationToken);
        if (!issueExists)
            throw new ArgumentException($"Задача с ID {command.IssueId} не найдена.");

        var author = await _userRepository.GetByIdAsync(command.AuthorId, cancellationToken);
        if (author == null)
            throw new ArgumentException($"Пользователь с ID {command.AuthorId} не найден.");

        var comment = Comment.CreateForIssue(
            command.IssueId,
            command.AuthorId,
            command.Text,
            command.IsInternal,
            command.Mentions
        );

        if (command.Attachments != null && command.Attachments.Any())
        {
            var attachmentsData = command.Attachments
        .Select(a => (a.Filename, a.FileUrl, a.FileSize, a.ContentType))
        .ToList();
            comment.AddAttachments(attachmentsData);
        }

        await _commentRepository.AddAsync(comment, cancellationToken);
        await _issueRepository.IncrementCommentCountAsync(command.IssueId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CommentResponseDto(
            comment.Id,
            comment.AuthorId,
            $"{author.Firstname} {author.Lastname}".Trim(),
            author.AvatarUrl,
            author.AvatarColor,
            comment.Text,
            comment.CreatedAt,
            comment.IsInternal,
            comment.Attachments.Select(a => new AttachmentResponseDto(
                a.Id,
                a.Filename,
                a.FileUrl,
                a.FileSize,
                a.ContentType,
                a.UploadedAt
            )).ToList()
        );
    }
}

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(x => x.IssueId).NotEmpty().WithMessage("Идентификатор задачи обязателен.");
        RuleFor(x => x.AuthorId).NotEmpty().WithMessage("Идентификатор автора обязателен.");
        RuleFor(x => x.Text).NotEmpty().WithMessage("Текст комментария не может быть пустым.")
                            .MaximumLength(4000).WithMessage("Комментарий слишком длинный (макс. 4000 символов).");

        RuleForEach(x => x.Attachments).ChildRules(attachment =>
        {
            attachment.RuleFor(a => a.Filename).NotEmpty().WithMessage("Имя файла обязательно.");
            attachment.RuleFor(a => a.FileUrl).NotEmpty().WithMessage("Ссылка на файл обязательна.")
                                              .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                                              .WithMessage("Ссылка на файл должна быть валидным URL.");
            attachment.RuleFor(a => a.FileSize).GreaterThan(0).WithMessage("Размер файла должен быть больше 0.");
            attachment.RuleFor(a => a.ContentType).NotEmpty().WithMessage("MIME-тип файла обязателен.");
        });
    }
}