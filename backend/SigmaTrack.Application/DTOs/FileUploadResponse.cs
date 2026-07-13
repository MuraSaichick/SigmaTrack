using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Application.DTOs
{
    public record FileUploadResponse(
        string Filename,
        string FileUrl,
        long FileSize,
        string ContentType);
}
