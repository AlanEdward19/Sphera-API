using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFieldsFromAuditEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Audit_Correlation",
                schema: "dbo",
                table: "AuditEntries");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                schema: "dbo",
                table: "AuditEntries");

            migrationBuilder.DropColumn(
                name: "Folder",
                schema: "dbo",
                table: "AuditEntries");

            migrationBuilder.DropColumn(
                name: "Path",
                schema: "dbo",
                table: "AuditEntries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CorrelationId",
                schema: "dbo",
                table: "AuditEntries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                schema: "dbo",
                table: "AuditEntries",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                schema: "dbo",
                table: "AuditEntries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Audit_Correlation",
                schema: "dbo",
                table: "AuditEntries",
                column: "CorrelationId");
        }
    }
}
