using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionSpecifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "productionSpecifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nomenclaturaIdOutput = table.Column<int>(type: "integer", nullable: false),
                    nomenclaturaIdInput = table.Column<int>(type: "integer", nullable: false),
                    quantityInput = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    quantityOutput = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productionSpecifications", x => x.id);
                    table.CheckConstraint("CK_productionSpecifications_quantityInput_positive", "\"quantityInput\" > 0");
                    table.CheckConstraint("CK_productionSpecifications_quantityOutput_positive", "\"quantityOutput\" > 0");
                    table.ForeignKey(
                        name: "FK_productionSpecifications_nomenclatura_nomenclaturaIdInput",
                        column: x => x.nomenclaturaIdInput,
                        principalTable: "nomenclatura",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productionSpecifications_nomenclatura_nomenclaturaIdOutput",
                        column: x => x.nomenclaturaIdOutput,
                        principalTable: "nomenclatura",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_productionSpecifications_nomenclaturaIdInput",
                table: "productionSpecifications",
                column: "nomenclaturaIdInput");

            migrationBuilder.CreateIndex(
                name: "IX_productionSpecifications_nomenclaturaIdOutput",
                table: "productionSpecifications",
                column: "nomenclaturaIdOutput");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "productionSpecifications");
        }
    }
}
