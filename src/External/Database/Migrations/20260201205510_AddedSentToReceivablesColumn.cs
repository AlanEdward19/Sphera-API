using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedSentToReceivablesColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceInstallment_Invoices_InvoiceId",
                schema: "dbo",
                table: "InvoiceInstallment");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceInstallment_InvoiceId",
                schema: "dbo",
                table: "InvoiceInstallment",
                newName: "IX_InvoiceInstallments_InvoiceId");

            migrationBuilder.AddColumn<bool>(
                name: "IsSentToReceivables",
                schema: "dbo",
                table: "Invoices",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                schema: "dbo",
                table: "InvoiceInstallment",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "dbo",
                table: "InvoiceInstallment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceInstallments_Invoices",
                schema: "dbo",
                table: "InvoiceInstallment",
                column: "InvoiceId",
                principalSchema: "dbo",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceInstallments_Invoices",
                schema: "dbo",
                table: "InvoiceInstallment");

            migrationBuilder.DropColumn(
                name: "IsSentToReceivables",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceInstallments_InvoiceId",
                schema: "dbo",
                table: "InvoiceInstallment",
                newName: "IX_InvoiceInstallment_InvoiceId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                schema: "dbo",
                table: "InvoiceInstallment",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "dbo",
                table: "InvoiceInstallment",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceInstallment_Invoices_InvoiceId",
                schema: "dbo",
                table: "InvoiceInstallment",
                column: "InvoiceId",
                principalSchema: "dbo",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
