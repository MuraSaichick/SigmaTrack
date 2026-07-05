using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;

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
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError);
        }
        private static async Task<IResult> UploadFileAsync(
            IFormFile file,
            IWebHostEnvironment env,
            HttpContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Results.BadRequest(new { message = "Файл не выбран или пуст." });
                var fileExtension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadsFolder = Path.Combine(env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream, cancellationToken);
                }
                var fileUrl = $"{context.Request.Scheme}://{context.Request.Host}/uploads/{uniqueFileName}";
                var response = new FileUploadResponse(
                    Filename: file.FileName,
                    FileUrl: fileUrl,
                    FileSize: file.Length,
                    ContentType: file.ContentType
                );

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: "Внутренняя ошибка сервера при загрузке файла.", statusCode: 500);
            }
        }
    }
    public record FileUploadResponse(
        string Filename,
        string FileUrl,
        long FileSize,
        string ContentType);
}
