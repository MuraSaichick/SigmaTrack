using SigmaTrack.Application.Interfaces;
using SigmaTrack.Application.Interfaces.Repositories;
using SigmaTrack.Domain.Enums;

namespace SigmaTrack.Application.Features.Profile.UploadAvatar
{
    public record UploadAvatarRequest(Guid UserId, Stream FileStream, string FileName, string ContentType);
    public record UploadAvatarResponse(string AvatarUrl);

    public interface IUploadAvatarUseCase
    {
        Task<UploadAvatarResponse> ExecuteAsync(UploadAvatarRequest request, CancellationToken cancellationToken = default);
    }

    public class UploadAvatarUseCase : IUploadAvatarUseCase
    {
        private readonly IStorageService _storageService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UploadAvatarUseCase(
            IStorageService storageService,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _storageService = storageService;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UploadAvatarResponse> ExecuteAsync(UploadAvatarRequest request, CancellationToken cancellationToken = default)
        {
            if (request.FileStream == null || request.FileStream.Length == 0)
                throw new ArgumentException("Файл аватара не может быть пустым.");

            string extension = Path.GetExtension(request.FileName).ToLowerInvariant();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".webp")
                throw new ArgumentException("Неподдерживаемый формат файла. Разрешены только JPG, PNG и WEBP.");

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {request.UserId} не найден.");
            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                await _storageService.DeleteFileAsync(user.AvatarUrl, cancellationToken);
            }
            var uploadResult = await _storageService.UploadFileAsync(
                request.FileStream,
                request.FileName,
                request.ContentType,
                StorageFolder.Avatars,
                cancellationToken
            );

            user.AvatarUrl = uploadResult.FileUrl;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UploadAvatarResponse(user.AvatarUrl);
        }
    }
}