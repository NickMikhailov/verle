using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddFusoEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FUSO",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fusoNumber = table.Column<string>(type: "text", nullable: false),
                    commissioningDate = table.Column<DateOnly>(type: "date", nullable: false),
                    capacity = table.Column<double>(type: "double precision", nullable: false),
                    unit = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FUSO", x => x.id);
                    table.ForeignKey(
                        name: "FK_FUSO_units_unit",
                        column: x => x.unit,
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FUSO_unit",
                table: "FUSO",
                column: "unit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FUSO");
        }
    }
}
