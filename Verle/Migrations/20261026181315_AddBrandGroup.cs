using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brandGroup",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brandGroup", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "brandBrandGroup",
                columns: table => new
                {
                    brandId = table.Column<int>(type: "integer", nullable: false),
                    brandGroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brandBrandGroup", x => x.brandId);
                    table.ForeignKey(
                        name: "FK_brandBrandGroup_brandGroup_brandGroupId",
                        column: x => x.brandGroupId,
                        principalTable: "brandGroup",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_brandBrandGroup_brand_brandId",
                        column: x => x.brandId,
                        principalTable: "brand",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_brandBrandGroup_brandGroupId",
                table: "brandBrandGroup",
                column: "brandGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "brandBrandGroup");

            migrationBuilder.DropTable(
                name: "brandGroup");
        }
    }
}
