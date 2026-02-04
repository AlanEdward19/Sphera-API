using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class BilletsAndRemittances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Remittances_BilletConfigurations_ConfigurationId",
                schema: "dbo",
                table: "Remittances");

            migrationBuilder.AddForeignKey(
                name: "FK_Remittances_BilletConfigurations",
                schema: "dbo",
                table: "Remittances",
                column: "ConfigurationId",
                principalSchema: "dbo",
                principalTable: "BilletConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Remittances_BilletConfigurations",
                schema: "dbo",
                table: "Remittances");

            migrationBuilder.AddForeignKey(
                name: "FK_Remittances_BilletConfigurations_ConfigurationId",
                schema: "dbo",
                table: "Remittances",
                column: "ConfigurationId",
                principalSchema: "dbo",
                principalTable: "BilletConfigurations",
                principalColumn: "Id");
        }
    }
}
