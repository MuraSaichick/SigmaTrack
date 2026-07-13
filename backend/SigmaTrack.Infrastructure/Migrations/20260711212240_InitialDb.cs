using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SigmaTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "global_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_global_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "project_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    login = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    hash_password = table.Column<string>(type: "text", nullable: false),
                    firstname = table.Column<string>(type: "text", nullable: false),
                    patronymic = table.Column<string>(type: "text", nullable: true),
                    lastname = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: true),
                    avatar_color = table.Column<string>(type: "text", nullable: true),
                    status_message = table.Column<string>(type: "text", nullable: true),
                    online_status = table.Column<int>(type: "integer", nullable: false),
                    bio = table.Column<string>(type: "text", nullable: true),
                    position = table.Column<string>(type: "text", nullable: true),
                    department = table.Column<string>(type: "text", nullable: true),
                    skills = table.Column<List<string>>(type: "text[]", nullable: false),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    telegram = table.Column<string>(type: "text", nullable: true),
                    git_hub = table.Column<string>(type: "text", nullable: true),
                    last_seen_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_global_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "global_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    link_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_notifications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    prefix = table.Column<string>(type: "text", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projects", x => x.id);
                    table.ForeignKey(
                        name: "fk_projects_users_creator_id",
                        column: x => x.creator_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "issue_templates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    component = table.Column<string>(type: "text", nullable: true),
                    tags = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'"),
                    is_default = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_templates", x => x.id);
                    table.ForeignKey(
                        name: "fk_issue_templates_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_custom_fields",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    options = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'"),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_custom_fields", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_custom_fields_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_invitations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invitee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    inviter_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_role_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    invited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    responded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_invitations", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_invitations_project_roles_project_role_id",
                        column: x => x.project_role_id,
                        principalTable: "project_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_project_invitations_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_project_invitations_users_invitee_id",
                        column: x => x.invitee_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_project_invitations_users_inviter_id",
                        column: x => x.inviter_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "project_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_role_id = table.Column<int>(type: "integer", nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_members_project_roles_project_role_id",
                        column: x => x.project_role_id,
                        principalTable: "project_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_project_members_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_project_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sprint",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    goal = table.Column<string>(type: "text", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    committed_points = table.Column<int>(type: "integer", nullable: false),
                    completed_points = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sprint", x => x.id);
                    table.ForeignKey(
                        name: "fk_sprint_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "issues",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    reporter_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assignee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    resolved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    story_points = table.Column<int>(type: "integer", nullable: true),
                    estimated_hours = table.Column<double>(type: "double precision", nullable: true),
                    logged_hours = table.Column<double>(type: "double precision", nullable: true),
                    remaining_hours = table.Column<double>(type: "double precision", nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: true),
                    component = table.Column<string>(type: "text", nullable: true),
                    version = table.Column<string>(type: "text", nullable: true),
                    fix_version = table.Column<string>(type: "text", nullable: true),
                    environment = table.Column<string>(type: "text", nullable: true),
                    tags = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'"),
                    sprint_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_reproducible = table.Column<bool>(type: "boolean", nullable: false),
                    steps_to_reproduce = table.Column<string>(type: "text", nullable: true),
                    is_blocked = table.Column<bool>(type: "boolean", nullable: false),
                    block_reason = table.Column<string>(type: "text", nullable: true),
                    blocked_by_issue_id = table.Column<Guid>(type: "uuid", nullable: true),
                    time_to_first_response = table.Column<TimeSpan>(type: "interval", nullable: true),
                    time_to_resolution = table.Column<TimeSpan>(type: "interval", nullable: true),
                    view_count = table.Column<int>(type: "integer", nullable: false),
                    comment_count = table.Column<int>(type: "integer", nullable: false),
                    source = table.Column<string>(type: "text", nullable: true),
                    custom_fields = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issues", x => x.id);
                    table.ForeignKey(
                        name: "fk_issues_issues_blocked_by_issue_id",
                        column: x => x.blocked_by_issue_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_issues_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_issues_sprint_sprint_id",
                        column: x => x.sprint_id,
                        principalTable: "sprint",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_issues_users_assignee_id",
                        column: x => x.assignee_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_issues_users_reporter_id",
                        column: x => x.reporter_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_internal = table.Column<bool>(type: "boolean", nullable: false),
                    mentions = table.Column<List<string>>(type: "text[]", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    issue_id = table.Column<Guid>(type: "uuid", nullable: true),
                    project_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_profile_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comments", x => x.id);
                    table.ForeignKey(
                        name: "fk_comments_issues_issue_id",
                        column: x => x.issue_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comments_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comments_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_comments_users_user_profile_id",
                        column: x => x.user_profile_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "issue_histories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    issue_id = table.Column<Guid>(type: "uuid", nullable: false),
                    changed_by = table.Column<Guid>(type: "uuid", nullable: false),
                    field_name = table.Column<string>(type: "text", nullable: false),
                    old_value = table.Column<string>(type: "text", nullable: true),
                    new_value = table.Column<string>(type: "text", nullable: true),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_issue_histories_issues_issue_id",
                        column: x => x.issue_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_issue_histories_users_changed_by",
                        column: x => x.changed_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_issue_histories_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "issue_links",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_issue_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_issue_id = table.Column<Guid>(type: "uuid", nullable: false),
                    link_type = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_links", x => x.id);
                    table.ForeignKey(
                        name: "fk_issue_links_issues_source_issue_id",
                        column: x => x.source_issue_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_issue_links_issues_target_issue_id",
                        column: x => x.target_issue_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_issue_links_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "issue_watchers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    issue_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    watched_since = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_watchers", x => x.id);
                    table.ForeignKey(
                        name: "fk_issue_watchers_issues_issue_id",
                        column: x => x.issue_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_issue_watchers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attachments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    filename = table.Column<string>(type: "text", nullable: false),
                    file_url = table.Column<string>(type: "text", nullable: false),
                    file_size = table.Column<long>(type: "bigint", nullable: false),
                    content_type = table.Column<string>(type: "text", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    comment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    issue_id = table.Column<Guid>(type: "uuid", nullable: true),
                    project_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attachments", x => x.id);
                    table.ForeignKey(
                        name: "fk_attachments_comments_comment_id",
                        column: x => x.comment_id,
                        principalTable: "comments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_attachments_issues_issue_id",
                        column: x => x.issue_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_attachments_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "global_roles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" }
                });

            migrationBuilder.InsertData(
                table: "project_roles",
                columns: new[] { "id", "description", "name" },
                values: new object[,]
                {
                    { 1, "Полный доступ к настройкам проекта, управлению участниками и всеми задачами.", "Project Manager" },
                    { 2, "Просмотр проекта, создание, редактирование и закрытие багов/задач.", "Developer" },
                    { 3, "Тестирование функционала, создание баг-репортов, проверка и верификация исправлений.", "QA Engineer" },
                    { 4, "Доступ только для чтения. Может просматривать задачи и оставлять комментарии без права редактирования.", "Observer" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_attachments_comment_id",
                table: "attachments",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "ix_attachments_issue_id",
                table: "attachments",
                column: "issue_id");

            migrationBuilder.CreateIndex(
                name: "ix_attachments_project_id",
                table: "attachments",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_author_id",
                table: "comments",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_issue_id",
                table: "comments",
                column: "issue_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_project_id",
                table: "comments",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_user_profile_id",
                table: "comments",
                column: "user_profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_histories_changed_by",
                table: "issue_histories",
                column: "changed_by");

            migrationBuilder.CreateIndex(
                name: "ix_issue_histories_issue_id",
                table: "issue_histories",
                column: "issue_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_histories_user_id",
                table: "issue_histories",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_links_created_by_user_id",
                table: "issue_links",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_links_source_issue_id",
                table: "issue_links",
                column: "source_issue_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_links_target_issue_id",
                table: "issue_links",
                column: "target_issue_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_templates_project_id",
                table: "issue_templates",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_watchers_issue_id",
                table: "issue_watchers",
                column: "issue_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_watchers_user_id",
                table: "issue_watchers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_issues_assignee_id",
                table: "issues",
                column: "assignee_id");

            migrationBuilder.CreateIndex(
                name: "ix_issues_blocked_by_issue_id",
                table: "issues",
                column: "blocked_by_issue_id");

            migrationBuilder.CreateIndex(
                name: "ix_issues_project_id",
                table: "issues",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_issues_reporter_id",
                table: "issues",
                column: "reporter_id");

            migrationBuilder.CreateIndex(
                name: "ix_issues_sprint_id",
                table: "issues",
                column: "sprint_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_user_id",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_custom_fields_project_id",
                table: "project_custom_fields",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_invitations_invitee_id",
                table: "project_invitations",
                column: "invitee_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_invitations_inviter_id",
                table: "project_invitations",
                column: "inviter_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_invitations_project_id",
                table: "project_invitations",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_invitations_project_role_id",
                table: "project_invitations",
                column: "project_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_members_project_id",
                table: "project_members",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_members_project_role_id",
                table: "project_members",
                column: "project_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_members_user_id",
                table: "project_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_creator_id",
                table: "projects",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_sprint_project_id",
                table: "sprint",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                table: "users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attachments");

            migrationBuilder.DropTable(
                name: "issue_histories");

            migrationBuilder.DropTable(
                name: "issue_links");

            migrationBuilder.DropTable(
                name: "issue_templates");

            migrationBuilder.DropTable(
                name: "issue_watchers");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "project_custom_fields");

            migrationBuilder.DropTable(
                name: "project_invitations");

            migrationBuilder.DropTable(
                name: "project_members");

            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "project_roles");

            migrationBuilder.DropTable(
                name: "issues");

            migrationBuilder.DropTable(
                name: "sprint");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "global_roles");
        }
    }
}
