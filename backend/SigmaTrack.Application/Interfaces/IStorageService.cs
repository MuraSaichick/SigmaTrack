using SigmaTrack.Application.DTOs;
using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.Interfaces
{
    public interface IStorageService
    {
        Task<FileUploadResponse> UploadFileAsync(
            Stream fileStream,
            string originalFileName,
            string contentType,
            StorageFolder folder,
            CancellationToken cancellationToken = default);

        Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    }
}
