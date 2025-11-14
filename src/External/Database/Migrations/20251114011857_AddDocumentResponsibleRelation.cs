using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sphera.API.External.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentResponsibleRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Documents_ResponsibleId",
                schema: "dbo",
                table: "Documents",
                column: "ResponsibleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Responsible",
                schema: "dbo",
                table: "Documents",
                column: "ResponsibleId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Responsible",
                schema: "dbo",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ResponsibleId",
                schema: "dbo",
                table: "Documents");
        }
    }
}
