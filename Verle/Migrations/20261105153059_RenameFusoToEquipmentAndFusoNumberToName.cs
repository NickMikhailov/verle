using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class RenameFusoToEquipmentAndFusoNumberToName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "FUSO",
                schema: "public",
                newName: "equipment");

            migrationBuilder.RenameColumn(
                name: "fusoNumber",
                table: "equipment",
                schema: "public",
                newName: "name");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "equipment",
                schema: "public",
                newName: "fusoNumber");

            migrationBuilder.RenameTable(
                name: "equipment",
                schema: "public",
                newName: "FUSO");
        }
    }
}
