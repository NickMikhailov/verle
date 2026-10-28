using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddNomenclaturaClassification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "nomenclaturaClass",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nomenclaturaClass", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "nomenclaturaClassLink",
                columns: table => new
                {
                    nomenclaturaId = table.Column<int>(type: "integer", nullable: false),
                    nomenclaturaClassId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nomenclaturaClassLink", x => x.nomenclaturaId);
                    table.ForeignKey(
                        name: "FK_nomenclaturaClassLink_nomenclaturaClass_nomenclaturaClassId",
                        column: x => x.nomenclaturaClassId,
                        principalTable: "nomenclaturaClass",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_nomenclaturaClassLink_nomenclatura_nomenclaturaId",
                        column: x => x.nomenclaturaId,
                        principalTable: "nomenclatura",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nomenclaturaClassLink_nomenclaturaClassId",
                table: "nomenclaturaClassLink",
                column: "nomenclaturaClassId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nomenclaturaClassLink");

            migrationBuilder.DropTable(
                name: "nomenclaturaClass");
        }
    }
}
