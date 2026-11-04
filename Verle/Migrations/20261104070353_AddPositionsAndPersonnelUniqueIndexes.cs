using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionsAndPersonnelUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_division_departmentId",
                table: "division");

            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    divisionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.id);
                    table.ForeignKey(
                        name: "FK_Position_division_divisionId",
                        column: x => x.divisionId,
                        principalTable: "division",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UX_Division_DepartmentId_Name",
                table: "division",
                columns: new[] { "departmentId", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Position_DivisionId_Name",
                table: "Position",
                columns: new[] { "divisionId", "name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Position");

            migrationBuilder.DropIndex(
                name: "UX_Division_DepartmentId_Name",
                table: "division");

            migrationBuilder.CreateIndex(
                name: "IX_division_departmentId",
                table: "division",
                column: "departmentId");
        }
    }
}
