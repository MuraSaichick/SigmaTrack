using SigmaTrack.Domain.Enums;
using SigmaTrack.Domain.Exceptions;
using System;
using System.Collections.Generic;

namespace SigmaTrack.Domain.Entities
{
    public class Issue
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public int Number { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public IssueStatus Status { get; set; }
        public IssueType Type { get; set; }
        public Guid ReporterId { get; set; }
        public User Reporter { get; set; } = null!;
        public Guid? AssigneeId { get; private set; }
        public User? Assignee { get; private set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DueDate { get; set; } // Срок выполнения
        public DateTime? StartedAt { get; set; } // Дата начала работы
        public DateTime? ResolvedAt { get; set; } // Дата решения
        public DateTime? ClosedAt { get; set; } // Дата закрытия
        public int? StoryPoints { get; set; } // Оценка в story points
        public double? EstimatedHours { get; set; } // Плановая оценка в часах
        public double? LoggedHours { get; set; } // Фактически затрачено
        public double? RemainingHours { get; set; } // Осталось часов
        public IssuePriority Priority { get; set; }
        public IssueSeverity? Severity { get; set; }
        public string? Component { get; set; } // Компонент системы
        public string? Version { get; set; } // Версия, где найден баг
        public string? FixVersion { get; set; } // Версия, где исправлен
        public string? Environment { get; set; } // Окружение
        public List<string> Tags { get; set; } = new(); // JSON-список тегов
        public Guid? SprintId { get; set; }
        public Sprint? Sprint { get; set; }
        public bool IsReproducible { get; set; } = true;
        public string? StepsToReproduce { get; set; }
        public bool IsBlocked { get; set; }
        public string? BlockReason { get; set; }
        public Guid? BlockedByIssueId { get; set; }
        public Issue? BlockedByIssue { get; set; }
        public TimeSpan? TimeToFirstResponse { get; set; } // Время до первого ответа
        public TimeSpan? TimeToResolution { get; set; } // Время до решения
        public int ViewCount { get; set; } // Количество просмотров
        public int CommentCount { get; set; } // Количество комментариев (денормализация)
        public string? Source { get; set; } // Источник задачи
        public Dictionary<string, string> CustomFields { get; set; } = new(); // Кастомные поля
        public ICollection<IssueComment> Comments { get; set; } = new List<IssueComment>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<IssueHistory> Histories { get; set; } = new List<IssueHistory>();
        public ICollection<IssueLink> SourceRelations { get; set; } = new List<IssueLink>();
        public ICollection<IssueLink> TargetRelations { get; set; } = new List<IssueLink>();
        public ICollection<IssueWatcher> Watchers { get; set; } = new List<IssueWatcher>();

        private Issue() { }
        public Issue(
            Guid projectId,
            int number,
            string title,
            string? description,
            IssueType type,
            Guid reporterId,
            IssuePriority priority,
            Guid? assigneeId = null,
            int? storyPoints = null,
            List<string>? tags = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Название задачи не может быть пустым.");
            if (number <= 0)
                throw new DomainException("Порядковый номер задачи должен быть больше нуля.");

            Id = Guid.NewGuid();
            ProjectId = projectId;
            Number = number;
            Title = title.Trim();
            Description = description;
            Type = type;
            ReporterId = reporterId;
            Priority = priority;
            AssigneeId = assigneeId;    
            StoryPoints = storyPoints;
            Tags = tags ?? new List<string>();
            Status = IssueStatus.Backlog;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public IssueComment AddComment(Guid authorId, string text, bool isInternal, List<string> mentions,
    List<(string Filename, string FileUrl, long FileSize, string ContentType)> attachmentsData)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Текст комментария не может быть пустым.");

            var comment = new IssueComment
            {
                Id = Guid.NewGuid(),
                IssueId = this.Id,
                AuthorId = authorId,
                Text = text.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsInternal = isInternal,
                Mentions = mentions ?? new List<string>()
            };

            if (attachmentsData != null)
            {
                foreach (var data in attachmentsData)
                {
                    comment.Attachments.Add(new CommentAttachment
                    {
                        Id = Guid.NewGuid(),
                        CommentId = comment.Id,
                        Filename = data.Filename,
                        FileUrl = data.FileUrl,
                        FileSize = data.FileSize,
                        ContentType = data.ContentType,
                        UploadedAt = DateTime.UtcNow
                    });
                }
            }

            Comments.Add(comment);
            CommentCount++;
            UpdatedAt = DateTime.UtcNow;
            return comment;
        }
        public IssueLink AddLink(Guid targetIssueId, IssueLinkType linkType, string? description, Guid userId)
        {
            if (targetIssueId == Id)
                throw new InvalidOperationException("Нельзя связать задачу с самой собой.");
            var link = new IssueLink
            {
                Id = Guid.NewGuid(),
                SourceIssueId = Id,
                TargetIssueId = targetIssueId,
                LinkType = linkType,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };
            SourceRelations.Add(link);

            Histories.Add(new IssueHistory
            {
                Id = Guid.NewGuid(),
                IssueId = Id,
                ChangedBy = userId,
                FieldName = "IssueLink",
                OldValue = null,
                NewValue = $"Создана связь [{linkType}] с задачей {targetIssueId}",
                ChangedAt = DateTime.UtcNow
            });

            UpdatedAt = DateTime.UtcNow;

            return link;
        }
        public void ChangeStatus(IssueStatus newStatus)
        {
            if (Status == newStatus)
                return;
            if (Status == IssueStatus.Closed && newStatus != IssueStatus.Reopened)
            {
                throw new DomainException($"Невозможно перевести задачу из статуса {Status} напрямую в {newStatus}. Сначала переоткройте её.");
            }
            if (newStatus == IssueStatus.InProgress && StartedAt == null)
            {
                StartedAt = DateTime.UtcNow;
            }
            else if (newStatus == IssueStatus.Resolved)
            {
                ResolvedAt = DateTime.UtcNow;
            }
            else if (newStatus == IssueStatus.Closed)
            {
                ClosedAt = DateTime.UtcNow;
            }
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateDetails(
            string title,
            string? description,
            IssuePriority priority,
            IssueSeverity? severity,
            DateTime? dueDate,
            double? estimatedHours,
            double? remainingHours,
            string? component,
            string? version,
            string? fixVersion,
            string? environment,
            List<string> tags,
            bool isReproducible,
            string? stepsToReproduce)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название задачи не может быть пустым.");
            if (estimatedHours < 0 || remainingHours < 0)
                throw new ArgumentException("Оценка часов не может быть отрицательной.");
            Title = title.Trim();
            Description = description?.Trim();
            Priority = priority;
            Severity = severity;
            DueDate = dueDate;
            EstimatedHours = estimatedHours;
            RemainingHours = remainingHours;
            Component = component?.Trim();
            Version = version?.Trim();
            FixVersion = fixVersion?.Trim();
            Environment = environment?.Trim();
            Tags = tags ?? new List<string>();
            IsReproducible = isReproducible;
            StepsToReproduce = stepsToReproduce?.Trim();

            UpdatedAt = DateTime.UtcNow;
        }
        public bool AssignTo(Guid? assigneeId)
        {
            if (AssigneeId == assigneeId)
                return false;

            AssigneeId = assigneeId;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
    }
}