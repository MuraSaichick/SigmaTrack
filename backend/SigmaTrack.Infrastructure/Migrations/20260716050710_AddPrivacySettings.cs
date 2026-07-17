using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigmaTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrivacySettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "privacy",
                table: "users",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{ \"ShowContacts\": 2, \"ShowBirthDate\": 2, \"ShowOnlineStatus\": true, \"WhoCanInviteMe\": 1, \"Searchable\": true, \"ShowStatusMessage\": true }'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "privacy",
                table: "users");
        }
    }
}
