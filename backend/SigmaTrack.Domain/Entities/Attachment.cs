using System;

namespace SigmaTrack.Domain.Entities
{
    public class Attachment
    {
        public Guid Id { get; private set; }
        public string Filename { get; private set; } = null!;
        public string FileUrl { get; private set; } = null!;
        public long FileSize { get; private set; }
        public string ContentType { get; private set; } = null!;
        public DateTime UploadedAt { get; private set; }

        public Guid? CommentId { get; private set; }
        public Guid? IssueId { get; private set; }
        public Guid? ProjectId { get; private set; }

        public Comment? Comment { get; private set; }
        public Issue? Issue { get; private set; }
        public Project? Project { get; private set; }

        private Attachment() { }

        public static Attachment Create(string filename, string fileUrl, long fileSize, string contentType)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Имя файла не может быть пустым.");

            if (string.IsNullOrWhiteSpace(fileUrl))
                throw new ArgumentException("Ссылка на файл не может быть пустой.");

            if (fileSize <= 0)
                throw new ArgumentException("Размер файла должен быть больше 0.");

            return new Attachment
            {
                Id = Guid.NewGuid(),
                Filename = filename.Trim(),
                FileUrl = fileUrl.Trim(),
                FileSize = fileSize,
                ContentType = contentType.Trim(),
                UploadedAt = DateTime.UtcNow
            };
        }
        internal void SetIds(Guid commentId, Guid issueId)
        {
            CommentId = commentId;
            IssueId = issueId;
        }
    }
}