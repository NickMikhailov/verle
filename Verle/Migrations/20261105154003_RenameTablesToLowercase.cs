using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesToLowercase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Department",
                schema: "public",
                newName: "department");

            migrationBuilder.RenameTable(
                name: "OrderStatuses",
                schema: "public",
                newName: "orderStatuses");

            migrationBuilder.RenameTable(
                name: "Position",
                schema: "public",
                newName: "position");

            migrationBuilder.RenameTable(
                name: "ProductionOrderExecutions",
                schema: "public",
                newName: "productionOrderExecutions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "department",
                schema: "public",
                newName: "Department");

            migrationBuilder.RenameTable(
                name: "orderStatuses",
                schema: "public",
                newName: "OrderStatuses");

            migrationBuilder.RenameTable(
                name: "position",
                schema: "public",
                newName: "Position");

            migrationBuilder.RenameTable(
                name: "productionOrderExecutions",
                schema: "public",
                newName: "ProductionOrderExecutions");
        }
    }
}
