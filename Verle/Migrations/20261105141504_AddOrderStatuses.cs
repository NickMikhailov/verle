using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "statusId",
                table: "DripSemiproductOrders",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateTable(
                name: "OrderStatuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatuses", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Редактируется" },
                    { 2, "Создан" },
                    { 3, "На исполнении" },
                    { 4, "На утверждении мастера" },
                    { 5, "На утверждении операционистом" },
                    { 6, "Выполнен" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DripSemiproductOrders_statusId",
                table: "DripSemiproductOrders",
                column: "statusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatuses_name",
                table: "OrderStatuses",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DripSemiproductOrders_OrderStatuses_statusId",
                table: "DripSemiproductOrders",
                column: "statusId",
                principalTable: "OrderStatuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DripSemiproductOrders_OrderStatuses_statusId",
                table: "DripSemiproductOrders");

            migrationBuilder.DropTable(
                name: "OrderStatuses");

            migrationBuilder.DropIndex(
                name: "IX_DripSemiproductOrders_statusId",
                table: "DripSemiproductOrders");

            migrationBuilder.DropColumn(
                name: "statusId",
                table: "DripSemiproductOrders");
        }
    }
}
