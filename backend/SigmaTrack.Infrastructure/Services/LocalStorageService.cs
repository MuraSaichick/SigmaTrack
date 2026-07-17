using Microsoft.Extensions.Configuration;
using SigmaTrack.Application.DTOs;
using SigmaTrack.Application.Interfaces;
using SigmaTrack.Domain.Enums;

namespace SigmaTrack.Infrastructure.Services
{
    public class LocalStorageService : IStorageService
    {
        private readonly string _storagePath;

        public LocalStorageService(IConfiguration configuration)
        {
            _storagePath = configuration["StorageSettings:LocalStoragePath"]
                           ?? throw new ArgumentNullException(nameof(configuration), "StorageSettings:LocalStoragePath не настроен");

            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task<FileUploadResponse> UploadFileAsync(
            Stream fileStream,
            string originalFileName,
            string contentType,
            StorageFolder folder,
            CancellationToken cancellationToken = default)
        {
            var fileExtension = Path.GetExtension(originalFileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var folderName = folder.ToString().ToLowerInvariant();
            var targetDirectory = Path.Combine(_storagePath, folderName);
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            var fullPath = Path.Combine(targetDirectory, uniqueFileName);
            using var targetStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
            await fileStream.CopyToAsync(targetStream, cancellationToken);
            var relativeUrl = $"/media/{folderName}/{uniqueFileName}";

            return new FileUploadResponse(
                Filename: originalFileName,
                FileUrl: relativeUrl,
                FileSize: fileStream.Length,
                ContentType: contentType
            );
        }

        public Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileUrl)) return Task.CompletedTask;
            var relativePath = fileUrl.Replace("/media/", "", StringComparison.OrdinalIgnoreCase);

            var fullPath = Path.Combine(_storagePath, relativePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }
    }
}