using System;
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

            migrationBuilder.CreateTable(
                name: "BilletConfigurations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    WalletNumber = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AgencyNumber = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    AccountDigit = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    HasFine = table.Column<bool>(type: "bit", nullable: false),
                    FinePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DailyDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailyInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountLimitDate = table.Column<DateTime>(type: "date", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RebateAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FirstMessage = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    SecondMessage = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    StartingSequentialNumber = table.Column<int>(type: "int", nullable: false),
                    StartingNossoNumero = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BilletConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Remittances",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Bank = table.Column<int>(type: "int", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    IsSubmitted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remittances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Remittances_BilletConfigurations",
                        column: x => x.ConfigurationId,
                        principalSchema: "dbo",
                        principalTable: "BilletConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Billets",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Bank = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InstallmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RemittanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Billets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Billets_BilletConfigurations",
                        column: x => x.ConfigurationId,
                        principalSchema: "dbo",
                        principalTable: "BilletConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Billets_Clients",
                        column: x => x.ClientId,
                        principalSchema: "dbo",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Billets_InvoiceInstallments",
                        column: x => x.InstallmentId,
                        principalSchema: "dbo",
                        principalTable: "InvoiceInstallment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Billets_Remittances",
                        column: x => x.RemittanceId,
                        principalSchema: "dbo",
                        principalTable: "Remittances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Billets_ClientId",
                schema: "dbo",
                table: "Billets",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Billets_ConfigurationId",
                schema: "dbo",
                table: "Billets",
                column: "ConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_Billets_InstallmentId",
                schema: "dbo",
                table: "Billets",
                column: "InstallmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Billets_RemittanceId",
                schema: "dbo",
                table: "Billets",
                column: "RemittanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Remittances_ConfigurationId",
                schema: "dbo",
                table: "Remittances",
                column: "ConfigurationId");

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

            migrationBuilder.DropTable(
                name: "Billets",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Remittances",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BilletConfigurations",
                schema: "dbo");

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
