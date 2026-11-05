using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class RenameProductionAndEmployeeTablesToSingular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "employees",
                schema: "public",
                newName: "employee");

            migrationBuilder.RenameTable(
                name: "orderStatuses",
                schema: "public",
                newName: "orderStatus");

            migrationBuilder.RenameTable(
                name: "productionOrderExecutions",
                schema: "public",
                newName: "productionOrderExecution");

            migrationBuilder.RenameTable(
                name: "productionOrders",
                schema: "public",
                newName: "productionOrder");

            migrationBuilder.RenameTable(
                name: "productionSpecifications",
                schema: "public",
                newName: "productionSpecification");

            migrationBuilder.RenameTable(
                name: "productionUnits",
                schema: "public",
                newName: "productionUnit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "productionUnit",
                schema: "public",
                newName: "productionUnits");

            migrationBuilder.RenameTable(
                name: "productionSpecification",
                schema: "public",
                newName: "productionSpecifications");

            migrationBuilder.RenameTable(
                name: "productionOrder",
                schema: "public",
                newName: "productionOrders");

            migrationBuilder.RenameTable(
                name: "productionOrderExecution",
                schema: "public",
                newName: "productionOrderExecutions");

            migrationBuilder.RenameTable(
                name: "orderStatus",
                schema: "public",
                newName: "orderStatuses");

            migrationBuilder.RenameTable(
                name: "employee",
                schema: "public",
                newName: "employees");
        }
    }
}
