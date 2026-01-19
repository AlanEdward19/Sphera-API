using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class ProgressStatusDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProgressStatus",
                schema: "dbo",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProgressStatus",
                schema: "dbo",
                table: "Documents");
        }
    }
}
