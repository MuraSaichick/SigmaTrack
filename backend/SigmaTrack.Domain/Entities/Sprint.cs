using SigmaTrack.Domain.Enums;
using SigmaTrack.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

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

        public void Complete()
        {
            if (Status != SprintStatus.Active)
            {
                throw new DomainException("Можно завершить только активный спринт.");
            }

            Status = SprintStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            CompletedPoints = Issues
                .Where(i => i.Status == IssueStatus.Closed && i.StoryPoints.HasValue)
                .Sum(i => i.StoryPoints!.Value);
        }
    }

}
