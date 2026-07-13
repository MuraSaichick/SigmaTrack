using FluentValidation;
using SigmaTrack.Application.Interfaces;
using SigmaTrack.Domain.Enums;

namespace SigmaTrack.Application.Features.Media.UploadMedia;

public record UploadMediaCommand(
    Stream FileStream,
    string OriginalFilename,
    string ContentType,
    long FileSize,
    StorageFolder Folder
);

public record UploadMediaResponseDto(
    string Filename,
    string FileUrl,
    long FileSize,
    string ContentType
);

public interface IUploadMediaUseCase
{
    Task<UploadMediaResponseDto> ExecuteAsync(UploadMediaCommand command, CancellationToken cancellationToken);
}

public class UploadMediaUseCase : IUploadMediaUseCase
{
    private readonly IStorageService _storageService;
    private readonly IValidator<UploadMediaCommand> _validator;

    public UploadMediaUseCase(IStorageService storageService, IValidator<UploadMediaCommand> validator)
    {
        _storageService = storageService;
        _validator = validator;
    }

    public async Task<UploadMediaResponseDto> ExecuteAsync(UploadMediaCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);
        var uploadResult = await _storageService.UploadFileAsync(
            command.FileStream,
            command.OriginalFilename,
            command.ContentType,
            command.Folder,
            cancellationToken
        );

        return new UploadMediaResponseDto(
            uploadResult.Filename,
            uploadResult.FileUrl,
            uploadResult.FileSize,
            uploadResult.ContentType
        );
    }
    public class UploadMediaCommandValidator : AbstractValidator<UploadMediaCommand>
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx", ".zip" };
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

        public UploadMediaCommandValidator()
        {
            RuleFor(x => x.FileStream)
                .NotNull().WithMessage("Файл пуст.");
            RuleFor(x => x.FileSize)
                .GreaterThan(0).WithMessage("Файл не должен быть пустым.")
                .LessThanOrEqualTo(MaxFileSize).WithMessage("Размер файла не должен превышать 10 МБ.");

            RuleFor(x => x.OriginalFilename)
                .NotEmpty().WithMessage("Имя файла обязательно.")
                .Must(filename => !string.IsNullOrEmpty(filename) && _allowedExtensions.Contains(Path.GetExtension(filename).ToLowerInvariant()))
                .WithMessage("Недопустимый формат файла.");
        }
    }
}