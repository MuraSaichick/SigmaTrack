using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigmaTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsUnderDocker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "link_type",
                table: "issue_links",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "link_type",
                table: "issue_links",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
