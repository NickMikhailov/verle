using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class RenameDripSemiproductOrdersToProductionOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DripSemiproductOrderExecutions");

            migrationBuilder.DropTable(
                name: "DripSemiproductOrders");

            migrationBuilder.CreateTable(
                name: "productOrders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id1C = table.Column<string>(type: "text", nullable: false),
                    orderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    nomenclaturaId = table.Column<int>(type: "integer", nullable: false),
                    shift = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    fusoId = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    plan = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    statusId = table.Column<int>(type: "integer", nullable: false, defaultValue: 2)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productOrders", x => x.id);
                    table.ForeignKey(
                        name: "FK_productOrders_FUSO_fusoId",
                        column: x => x.fusoId,
                        principalTable: "FUSO",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productOrders_OrderStatuses_statusId",
                        column: x => x.statusId,
                        principalTable: "OrderStatuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productOrders_nomenclatura_nomenclaturaId",
                        column: x => x.nomenclaturaId,
                        principalTable: "nomenclatura",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productOrders_shifts_shift",
                        column: x => x.shift,
                        principalTable: "shifts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductionOrderExecutions",
                columns: table => new
                {
                    orderId = table.Column<int>(type: "integer", nullable: false),
                    actual = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionOrderExecutions", x => x.orderId);
                    table.ForeignKey(
                        name: "FK_ProductionOrderExecutions_productOrders_orderId",
                        column: x => x.orderId,
                        principalTable: "productOrders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_productOrders_fusoId",
                table: "productOrders",
                column: "fusoId");

            migrationBuilder.CreateIndex(
                name: "IX_productOrders_nomenclaturaId",
                table: "productOrders",
                column: "nomenclaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_productOrders_shift",
                table: "productOrders",
                column: "shift");

            migrationBuilder.CreateIndex(
                name: "IX_productOrders_statusId",
                table: "productOrders",
                column: "statusId");

            migrationBuilder.CreateIndex(
                name: "UX_DripSemiproductOrders_id1C",
                table: "productOrders",
                column: "id1C",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionOrderExecutions");

            migrationBuilder.DropTable(
                name: "productOrders");

            migrationBuilder.CreateTable(
                name: "DripSemiproductOrders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fusoId = table.Column<int>(type: "integer", nullable: false),
                    nomenclaturaId = table.Column<int>(type: "integer", nullable: false),
                    shift = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    statusId = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    comment = table.Column<string>(type: "text", nullable: true),
                    id1C = table.Column<string>(type: "text", nullable: false),
                    orderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    plan = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DripSemiproductOrders", x => x.id);
                    table.ForeignKey(
                        name: "FK_DripSemiproductOrders_FUSO_fusoId",
                        column: x => x.fusoId,
                        principalTable: "FUSO",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DripSemiproductOrders_OrderStatuses_statusId",
                        column: x => x.statusId,
                        principalTable: "OrderStatuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DripSemiproductOrders_nomenclatura_nomenclaturaId",
                        column: x => x.nomenclaturaId,
                        principalTable: "nomenclatura",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DripSemiproductOrders_shifts_shift",
                        column: x => x.shift,
                        principalTable: "shifts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_DripSemiproductOrders_fusoId",
                table: "DripSemiproductOrders",
                column: "fusoId");

            migrationBuilder.CreateIndex(
                name: "IX_DripSemiproductOrders_nomenclaturaId",
                table: "DripSemiproductOrders",
                column: "nomenclaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_DripSemiproductOrders_shift",
                table: "DripSemiproductOrders",
                column: "shift");

            migrationBuilder.CreateIndex(
                name: "IX_DripSemiproductOrders_statusId",
                table: "DripSemiproductOrders",
                column: "statusId");

            migrationBuilder.CreateIndex(
                name: "UX_DripSemiproductOrders_id1C",
                table: "DripSemiproductOrders",
                column: "id1C",
                unique: true);
        }
    }
}
