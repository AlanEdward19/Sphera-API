using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldParamAddNewParamDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlobUri",
                schema: "dbo",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ContentType",
                schema: "dbo",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FileSize",
                schema: "dbo",
                table: "Documents");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                schema: "dbo",
                table: "Documents",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(260)",
                oldMaxLength: 260,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                schema: "dbo",
                table: "Documents",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(260)",
                oldMaxLength: 260);

            migrationBuilder.AddColumn<string>(
                name: "BlobUri",
                schema: "dbo",
                table: "Documents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                schema: "dbo",
                table: "Documents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                schema: "dbo",
                table: "Documents",
                type: "bigint",
                nullable: true);
        }
    }
}
