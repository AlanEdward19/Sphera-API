using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sphera.API.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedIdToRoleSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { (short)1, new DateTime(2025, 11, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Administrador", null },
                    { (short)2, new DateTime(2025, 11, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gestor", null },
                    { (short)3, new DateTime(2025, 11, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Financeiro", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Roles",
                keyColumn: "Id",
                keyValue: (short)1);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Roles",
                keyColumn: "Id",
                keyValue: (short)2);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Roles",
                keyColumn: "Id",
                keyValue: (short)3);
        }
    }
}
