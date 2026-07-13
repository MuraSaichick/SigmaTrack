using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigmaTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addFeatureSprint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_issues_sprint_sprint_id",
                table: "issues");

            migrationBuilder.DropForeignKey(
                name: "fk_sprint_projects_project_id",
                table: "sprint");

            migrationBuilder.DropPrimaryKey(
                name: "pk_sprint",
                table: "sprint");

            migrationBuilder.RenameTable(
                name: "sprint",
                newName: "sprints");

            migrationBuilder.RenameIndex(
                name: "ix_sprint_project_id",
                table: "sprints",
                newName: "ix_sprints_project_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_sprints",
                table: "sprints",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_issues_sprints_sprint_id",
                table: "issues",
                column: "sprint_id",
                principalTable: "sprints",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_sprints_projects_project_id",
                table: "sprints",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_issues_sprints_sprint_id",
                table: "issues");

            migrationBuilder.DropForeignKey(
                name: "fk_sprints_projects_project_id",
                table: "sprints");

            migrationBuilder.DropPrimaryKey(
                name: "pk_sprints",
                table: "sprints");

            migrationBuilder.RenameTable(
                name: "sprints",
                newName: "sprint");

            migrationBuilder.RenameIndex(
                name: "ix_sprints_project_id",
                table: "sprint",
                newName: "ix_sprint_project_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_sprint",
                table: "sprint",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_issues_sprint_sprint_id",
                table: "issues",
                column: "sprint_id",
                principalTable: "sprint",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_sprint_projects_project_id",
                table: "sprint",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
