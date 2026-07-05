using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SigmaTrack.Domain.Entities;
using SigmaTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SigmaTrack.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<GlobalRole> GlobalRoles => Set<GlobalRole>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
        public DbSet<ProjectRole> ProjectRoles => Set<ProjectRole>();
        public DbSet<Issue> Issues => Set<Issue>();
        public DbSet<IssueLink> IssueLinks => Set<IssueLink>();
        public DbSet<IssueComment> Comments => Set<IssueComment>();
        public DbSet<CommentAttachment> CommentAttachments => Set<CommentAttachment>();
        public DbSet<Attachment> Attachments => Set<Attachment>();
        public DbSet<IssueHistory> IssueHistories => Set<IssueHistory>();
        public DbSet<IssueWatcher> IssueWatchers => Set<IssueWatcher>();
        public DbSet<IssueTemplate> IssueTemplates => Set<IssueTemplate>();
        public DbSet<ProjectCustomField> ProjectCustomFields => Set<ProjectCustomField>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<ProjectInvitation> ProjectInvitations => Set<ProjectInvitation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<GlobalRole>().HasKey(gr => gr.Id);
            modelBuilder.Entity<Project>().HasKey(p => p.Id);
            modelBuilder.Entity<ProjectRole>().HasKey(pr => pr.Id);
            modelBuilder.Entity<ProjectMember>().HasKey(pm => pm.Id);
            modelBuilder.Entity<Issue>().HasKey(i => i.Id);
            modelBuilder.Entity<IssueLink>().HasKey(il => il.Id);
            modelBuilder.Entity<IssueComment>().HasKey(c => c.Id);
            modelBuilder.Entity<CommentAttachment>().HasKey(ca => ca.Id);
            modelBuilder.Entity<Attachment>().HasKey(a => a.Id);
            modelBuilder.Entity<IssueHistory>().HasKey(ih => ih.Id);
            modelBuilder.Entity<IssueWatcher>().HasKey(iw => iw.Id);
            modelBuilder.Entity<IssueTemplate>().HasKey(it => it.Id);
            modelBuilder.Entity<ProjectCustomField>().HasKey(pcf => pcf.Id);
            modelBuilder.Entity<Notification>().HasKey(n => n.Id);
            modelBuilder.Entity<ProjectInvitation>().HasKey(pi => pi.Id);

            var listComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
                );
            var dictComparer = new ValueComparer<Dictionary<string, string>>(
                (d1, d2) => d1!.Count == d2!.Count && !d1.Except(d2).Any(),
                d => d.Aggregate(0, (a, p) => HashCode.Combine(a, p.Key.GetHashCode(), p.Value.GetHashCode())),
                d => d.ToDictionary(k => k.Key, v => v.Value)
            );

            modelBuilder.Entity<Issue>(entity =>
            {
                var listComparer = new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                );
                var dictComparer = new ValueComparer<Dictionary<string, string>>(
                    (d1, d2) => d1!.Count == d2!.Count && !d1.Except(d2).Any(),
                    d => d.Aggregate(0, (a, p) => HashCode.Combine(a, p.Key.GetHashCode(), p.Value.GetHashCode())),
                    d => d.ToDictionary(k => k.Key, v => v.Value)
                );

                entity.Property(e => e.Tags)
                    .HasColumnType("jsonb")
                    .HasDefaultValueSql("'[]'")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                    )
                    .Metadata.SetValueComparer(listComparer);

                entity.Property(e => e.CustomFields)
                    .HasColumnType("jsonb")
                    .HasDefaultValueSql("'{}'")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>()
                    )
                    .Metadata.SetValueComparer(dictComparer);
            });

            modelBuilder.Entity<IssueTemplate>(entity =>
            {
                var listComparer = new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                );

                entity.Property(e => e.Tags)
                    .HasColumnType("jsonb")
                    .HasDefaultValueSql("'[]'")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                    )
                    .Metadata.SetValueComparer(listComparer);
            });

            modelBuilder.Entity<ProjectCustomField>(entity =>
            {
                var listComparer = new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                );

                entity.Property(e => e.Options)
                    .HasColumnType("jsonb")
                    .HasDefaultValueSql("'[]'")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                    )
                    .Metadata.SetValueComparer(listComparer);
            });


            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Project>()
                .HasOne(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

 
            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project)
                .WithMany(p => p.ProjectMembers)
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.User)
                .WithMany(u => u.ProjectMembers)
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.ProjectRole)
                .WithMany(pr => pr.ProjectMembers)
                .HasForeignKey(pm => pm.ProjectRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Issue>()
                .HasOne(i => i.Reporter)
                .WithMany()
                .HasForeignKey(i => i.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Issue>()
                .HasOne(i => i.Assignee)
                .WithMany()
                .HasForeignKey(i => i.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Issue>()
                .HasOne(i => i.Project)
                .WithMany(p => p.Issues)
                .HasForeignKey(i => i.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Issue>()
                .HasOne(i => i.BlockedByIssue)
                .WithMany()
                .HasForeignKey(i => i.BlockedByIssueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IssueLink>()
                .HasOne(il => il.SourceIssue)
                .WithMany(i => i.SourceRelations)
                .HasForeignKey(il => il.SourceIssueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IssueLink>()
                .HasOne(il => il.TargetIssue)
                .WithMany(i => i.TargetRelations)
                .HasForeignKey(il => il.TargetIssueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IssueComment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IssueComment>()
                .HasOne(c => c.Issue)
                .WithMany(i => i.Comments)
                .HasForeignKey(c => c.IssueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommentAttachment>()
                .HasOne(ca => ca.Comment)
                .WithMany()
                .HasForeignKey(ca => ca.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.Issue)
                .WithMany(i => i.Attachments)
                .HasForeignKey(a => a.IssueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IssueHistory>()
                .HasOne(ih => ih.Issue)
                .WithMany(i => i.Histories)
                .HasForeignKey(ih => ih.IssueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IssueHistory>()
                .HasOne(ih => ih.User)
                .WithMany()
                .HasForeignKey(ih => ih.ChangedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IssueWatcher>()
                .HasOne(iw => iw.Issue)
                .WithMany(i => i.Watchers)
                .HasForeignKey(iw => iw.IssueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IssueWatcher>()
                .HasOne(iw => iw.User)
                .WithMany()
                .HasForeignKey(iw => iw.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IssueTemplate>()
                .HasOne(it => it.Project)
                .WithMany()
                .HasForeignKey(it => it.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectCustomField>()
                .HasOne(pcf => pcf.Project)
                .WithMany()
                .HasForeignKey(pcf => pcf.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectInvitation>()
                .HasOne(pi => pi.Project)
                .WithMany(p => p.Invitations)
                .HasForeignKey(pi => pi.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectInvitation>()
                .HasOne(pi => pi.Invitee)
                .WithMany(u => u.IncomingInvitations)
                .HasForeignKey(pi => pi.InviteeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectInvitation>()
                .HasOne(pi => pi.Inviter)
                .WithMany(u => u.OutgoingInvitations)
                .HasForeignKey(pi => pi.InviterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GlobalRole>().HasData(
                new GlobalRole { Id = (int)GlobalRoleEnum.Admin, Name = "Admin" },
                new GlobalRole { Id = (int)GlobalRoleEnum.User, Name = "User" }
            );

            modelBuilder.Entity<ProjectRole>().HasData(
                new ProjectRole
                {
                    Id = (int)ProjectRoleEnum.ProjectManager,
                    Name = "Project Manager",
                    Description = "Полный доступ к настройкам проекта, управлению участниками и всеми задачами."
                },
                new ProjectRole
                {
                    Id = (int)ProjectRoleEnum.Developer,
                    Name = "Developer",
                    Description = "Просмотр проекта, создание, редактирование и закрытие багов/задач."
                },
                new ProjectRole
                {
                    Id = (int)ProjectRoleEnum.QAEngineer,
                    Name = "QA Engineer",
                    Description = "Тестирование функционала, создание баг-репортов, проверка и верификация исправлений."
                },
                new ProjectRole
                {
                    Id = (int)ProjectRoleEnum.Observer,
                    Name = "Observer",
                    Description = "Доступ только для чтения. Может просматривать задачи и оставлять комментарии без права редактирования."
                }
            );
        }
    }
}