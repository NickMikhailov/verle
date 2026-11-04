using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddDripSemiproductOrderExecutions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DripSemiproductOrderExecutions",
                columns: table => new
                {
                    orderId = table.Column<int>(type: "integer", nullable: false),
                    actual = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DripSemiproductOrderExecutions", x => x.orderId);
                    table.ForeignKey(
                        name: "FK_DripSemiproductOrderExecutions_DripSemiproductOrders_orderId",
                        column: x => x.orderId,
                        principalTable: "DripSemiproductOrders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DripSemiproductOrderExecutions");
        }
    }
}
