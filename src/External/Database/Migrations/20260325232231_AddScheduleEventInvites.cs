using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleEventInvites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleEventInvites",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ScheduleEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvitedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleEventInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleEventInvites_ScheduleEvent",
                        column: x => x.ScheduleEventId,
                        principalSchema: "dbo",
                        principalTable: "ScheduleEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleEventInvites_User",
                        column: x => x.InvitedUserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEventInvites_InvitedUserId",
                schema: "dbo",
                table: "ScheduleEventInvites",
                column: "InvitedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEventInvites_ScheduleEventId",
                schema: "dbo",
                table: "ScheduleEventInvites",
                column: "ScheduleEventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleEventInvites",
                schema: "dbo");
        }
    }
}
