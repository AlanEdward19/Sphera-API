using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedIdToRoleSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Roles",
                keyColumn: "Id",
                keyValue: (short)1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 26, 4, 14, 7, 118, DateTimeKind.Utc).AddTicks(528), new DateTime(2025, 11, 26, 4, 14, 7, 118, DateTimeKind.Utc).AddTicks(761) });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Roles",
                keyColumn: "Id",
                keyValue: (short)2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 26, 4, 14, 7, 118, DateTimeKind.Utc).AddTicks(1270), new DateTime(2025, 11, 26, 4, 14, 7, 118, DateTimeKind.Utc).AddTicks(1271) });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Roles",
                keyColumn: "Id",
                keyValue: (short)3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 26, 4, 14, 7, 118, DateTimeKind.Utc).AddTicks(1273), new DateTime(2025, 11, 26, 4, 14, 7, 118, DateTimeKind.Utc).AddTicks(1273) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Roles",
                keyColumn: "Id",
                keyValue: (short)1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 26, 4, 13, 8, 834, DateTimeKind.Utc).AddTicks(3165), new DateTime(2025, 11, 26, 4, 13, 8, 834, DateTimeKind.Utc).AddTicks(3401) });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Roles",
                keyColumn: "Id",
                keyValue: (short)2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 26, 4, 13, 8, 834, DateTimeKind.Utc).AddTicks(4137), new DateTime(2025, 11, 26, 4, 13, 8, 834, DateTimeKind.Utc).AddTicks(4138) });

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Roles",
                keyColumn: "Id",
                keyValue: (short)3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 26, 4, 13, 8, 834, DateTimeKind.Utc).AddTicks(4140), new DateTime(2025, 11, 26, 4, 13, 8, 834, DateTimeKind.Utc).AddTicks(4140) });
        }
    }
}
