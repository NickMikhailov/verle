using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddDripSemiproductOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DripSemiproductOrders",
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
                name: "UX_DripSemiproductOrders_id1C",
                table: "DripSemiproductOrders",
                column: "id1C",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DripSemiproductOrders");
        }
    }
}
