using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "id1C",
                table: "nomenclatura",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "sales",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    counterpartId = table.Column<int>(type: "integer", nullable: false),
                    counterpartId1C = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    counterpartName1C = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    document = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    nomenclaturaId = table.Column<int>(type: "integer", nullable: false),
                    nomenclaturaId1C = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nomenclaturaName1C = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    revenue = table.Column<double>(type: "double precision", nullable: false),
                    cost = table.Column<double>(type: "double precision", nullable: false),
                    grossProfit = table.Column<double>(type: "double precision", nullable: false),
                    margin = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sales", x => x.id);
                    table.ForeignKey(
                        name: "FK_sales_counterpart_counterpartId",
                        column: x => x.counterpartId,
                        principalTable: "counterpart",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sales_nomenclatura_nomenclaturaId",
                        column: x => x.nomenclaturaId,
                        principalTable: "nomenclatura",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ux_nomenclatura_id1c",
                table: "nomenclatura",
                column: "id1C",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sales_counterpartId",
                table: "sales",
                column: "counterpartId");

            migrationBuilder.CreateIndex(
                name: "IX_sales_date",
                table: "sales",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_sales_nomenclaturaId",
                table: "sales",
                column: "nomenclaturaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sales");

            migrationBuilder.DropIndex(
                name: "ux_nomenclatura_id1c",
                table: "nomenclatura");

            migrationBuilder.AlterColumn<string>(
                name: "id1C",
                table: "nomenclatura",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
