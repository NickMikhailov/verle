using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "productionUnits",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productionUnits", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "productionUnits",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Обжарка" },
                    { 2, "Фасовка зерна (ручка)" },
                    { 3, "Фасовка зерна (СИА)" },
                    { 4, "ПФ капсулы" },
                    { 5, "ПФ Дрипы" },
                    { 6, "Сборка ГП" }
                });

            migrationBuilder.CreateIndex(
                name: "UX_productionUnits_name",
                table: "productionUnits",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "productionUnits");
        }
    }
}
