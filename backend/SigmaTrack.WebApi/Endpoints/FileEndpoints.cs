using Microsoft.AspNetCore.Mvc;
using SigmaTrack.Application.DTOs;
using SigmaTrack.Application.Features.Media.UploadMedia;
using SigmaTrack.Application.Interfaces;
using SigmaTrack.Domain.Enums;


namespace SigmaTrack.WebApi.Endpoints.Files
{
    public class FileEndpoints : IEndpointModule
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/files").WithTags("Files");

            group.MapPost("upload", UploadFileAsync)
                .WithName("UploadFile")
                .DisableAntiforgery()
                .Produces<FileUploadResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);
        }

        private static async Task<IResult> UploadFileAsync(
    IFormFile file,
    IUploadMediaUseCase uploadMediaUseCase,
    [FromQuery] StorageFolder folder,
    CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest(new { message = "Файл не выбран или пуст." });
            using var stream = file.OpenReadStream();
            var command = new UploadMediaCommand(
                FileStream: stream,
                OriginalFilename: file.FileName,
                ContentType: file.ContentType,
                FileSize: file.Length,
                Folder: folder
            );

            var response = await uploadMediaUseCase.ExecuteAsync(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}