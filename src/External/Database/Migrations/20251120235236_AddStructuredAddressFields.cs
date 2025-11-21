using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStructuredAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Complement",
                schema: "dbo",
                table: "Partners",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lot",
                schema: "dbo",
                table: "Partners",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                schema: "dbo",
                table: "Partners",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Complement",
                schema: "dbo",
                table: "Clients",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lot",
                schema: "dbo",
                table: "Clients",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                schema: "dbo",
                table: "Clients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Complement",
                schema: "dbo",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Lot",
                schema: "dbo",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                schema: "dbo",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Complement",
                schema: "dbo",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Lot",
                schema: "dbo",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                schema: "dbo",
                table: "Clients");
        }
    }
}
