using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPartnersProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingDueDay",
                schema: "dbo",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "MunicipalRegistration",
                schema: "dbo",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "StateRegistration",
                schema: "dbo",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "TradeName",
                schema: "dbo",
                table: "Partners");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "BillingDueDay",
                schema: "dbo",
                table: "Partners",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MunicipalRegistration",
                schema: "dbo",
                table: "Partners",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateRegistration",
                schema: "dbo",
                table: "Partners",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TradeName",
                schema: "dbo",
                table: "Partners",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "");
        }
    }
}
