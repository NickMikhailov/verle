using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddNomenclaturaAdditionalClassifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brand",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brand", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "productType",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productType", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "skuClass",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skuClass", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "nomenclaturaBrand",
                columns: table => new
                {
                    nomenclaturaId = table.Column<int>(type: "integer", nullable: false),
                    brandId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nomenclaturaBrand", x => x.nomenclaturaId);
                    table.ForeignKey(
                        name: "FK_nomenclaturaBrand_brand_brandId",
                        column: x => x.brandId,
                        principalTable: "brand",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_nomenclaturaBrand_nomenclatura_nomenclaturaId",
                        column: x => x.nomenclaturaId,
                        principalTable: "nomenclatura",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "nomenclaturaProductType",
                columns: table => new
                {
                    nomenclaturaId = table.Column<int>(type: "integer", nullable: false),
                    productTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nomenclaturaProductType", x => x.nomenclaturaId);
                    table.ForeignKey(
                        name: "FK_nomenclaturaProductType_nomenclatura_nomenclaturaId",
                        column: x => x.nomenclaturaId,
                        principalTable: "nomenclatura",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_nomenclaturaProductType_productType_productTypeId",
                        column: x => x.productTypeId,
                        principalTable: "productType",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "nomenclaturaSkuClass",
                columns: table => new
                {
                    nomenclaturaId = table.Column<int>(type: "integer", nullable: false),
                    skuClassId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nomenclaturaSkuClass", x => x.nomenclaturaId);
                    table.ForeignKey(
                        name: "FK_nomenclaturaSkuClass_nomenclatura_nomenclaturaId",
                        column: x => x.nomenclaturaId,
                        principalTable: "nomenclatura",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_nomenclaturaSkuClass_skuClass_skuClassId",
                        column: x => x.skuClassId,
                        principalTable: "skuClass",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nomenclaturaBrand_brandId",
                table: "nomenclaturaBrand",
                column: "brandId");

            migrationBuilder.CreateIndex(
                name: "IX_nomenclaturaProductType_productTypeId",
                table: "nomenclaturaProductType",
                column: "productTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_nomenclaturaSkuClass_skuClassId",
                table: "nomenclaturaSkuClass",
                column: "skuClassId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nomenclaturaBrand");

            migrationBuilder.DropTable(
                name: "nomenclaturaProductType");

            migrationBuilder.DropTable(
                name: "nomenclaturaSkuClass");

            migrationBuilder.DropTable(
                name: "brand");

            migrationBuilder.DropTable(
                name: "productType");

            migrationBuilder.DropTable(
                name: "skuClass");
        }
    }
}
