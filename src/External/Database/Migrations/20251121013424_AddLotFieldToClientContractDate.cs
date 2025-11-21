using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLotFieldToClientContractDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ContractDate",
                schema: "dbo",
                table: "Clients",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractDate",
                schema: "dbo",
                table: "Clients");
        }
    }
}
