using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedEntityNameToAuditory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntityName",
                schema: "dbo",
                table: "AuditEntries",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityName",
                schema: "dbo",
                table: "AuditEntries");
        }
    }
}
