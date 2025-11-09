using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class serviceWithDateInsteadOfNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultDueInDays",
                schema: "dbo",
                table: "Services");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                schema: "dbo",
                table: "Services",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                schema: "dbo",
                table: "Services");

            migrationBuilder.AddColumn<short>(
                name: "DefaultDueInDays",
                schema: "dbo",
                table: "Services",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
