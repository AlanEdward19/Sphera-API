using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLotFieldToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhoneType",
                schema: "dbo",
                table: "Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "dbo",
                table: "Contacts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UserId",
                schema: "dbo",
                table: "Contacts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Audit_Actor",
                schema: "dbo",
                table: "AuditEntries",
                column: "ActorId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_User",
                schema: "dbo",
                table: "Contacts",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audit_Actor",
                schema: "dbo",
                table: "AuditEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_User",
                schema: "dbo",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_UserId",
                schema: "dbo",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "PhoneType",
                schema: "dbo",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "Contacts");
        }
    }
}
