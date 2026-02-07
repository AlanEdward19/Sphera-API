using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class BilletOurNumberColumnAndNewBilletConfigurationColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NossoNumero",
                schema: "dbo",
                table: "Billets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                schema: "dbo",
                table: "BilletConfigurations",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NossoNumero",
                schema: "dbo",
                table: "Billets");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "dbo",
                table: "BilletConfigurations");
        }
    }
}
