using SigmaTrack.Domain.Enums;
using SigmaTrack.Domain.Exceptions;

namespace SigmaTrack.Domain.Entities
{
    public class Sprint
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Goal { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SprintStatus Status { get; set; }   
        public int Capacity { get; set; }               // Общая емкость спринта в story points
        public int CommittedPoints { get; set; }        
        public int CompletedPoints { get; set; }        
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public ICollection<Issue> Issues { get; set; } = new List<Issue>();

        private Sprint() { }

        public Sprint(Guid projectId, string name, string? goal, DateTime startDate, DateTime endDate, int capacity)
        {
            if (startDate >= endDate)
                throw new DomainException("Дата начала спринта должна быть раньше даты окончания.");

            Id = Guid.NewGuid();
            ProjectId = projectId;
            Name = name;
            Goal = goal;
            StartDate = startDate;
            EndDate = endDate;
            Capacity = capacity;
            Status = SprintStatus.Planning;
            CreatedAt = DateTime.UtcNow;
        }
        public void Start()
        {
            if (Status != SprintStatus.Planning)
            {
                throw new DomainException("Можно запустить только спринт в статусе Планирование.");
            }

            Status = SprintStatus.Active;
        }
        public void RecalculateCommittedPoints()
        {
            CommittedPoints = Issues
                .Where(i => i.StoryPoints.HasValue)
                .Sum(i => i.StoryPoints!.Value);
        }
        public void Complete()
        {
            if (Status != SprintStatus.Active)
            {
                throw new DomainException("Можно завершить только активный спринт.");
            }

            Status = SprintStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            UpdateMetrics();
        }

        public void Cancel()
        {
            if (Status != SprintStatus.Active && Status != SprintStatus.Planning)
            {
                throw new DomainException("Можно отменить только активный или планируемый спринт.");
            }

            Status = SprintStatus.Cancelled;
            CompletedAt = DateTime.UtcNow;
            UpdateMetrics();
            var activeIssues = Issues
                .Where(i => i.Status != IssueStatus.Closed && i.Status != IssueStatus.Resolved)
                .ToList();
            foreach (var issue in activeIssues)
            {
                Issues.Remove(issue);
            }
        }
        private void UpdateMetrics()
        {
            RecalculateCommittedPoints();
            CompletedPoints = Issues
                .Where(i => (i.Status == IssueStatus.Closed || i.Status == IssueStatus.Resolved) && i.StoryPoints.HasValue)
                .Sum(i => i.StoryPoints!.Value);
        }
        public void RemoveIssue(Issue issue)
        {
            if (Status == SprintStatus.Completed || Status == SprintStatus.Cancelled)
            {
                throw new DomainException("Нельзя удалять задачи из завершенного или отмененного спринта.");
            }

            if (!Issues.Contains(issue))
            {
                throw new DomainException("Эта задача не принадлежит данному спринту.");
            }
            Issues.Remove(issue);
            RecalculateCommittedPoints();
        }
    }
}