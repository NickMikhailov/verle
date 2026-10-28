using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddCounterpartGroupAndLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "counterpartGroup",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_counterpartGroup", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "counterpartCounterpartGroup",
                columns: table => new
                {
                    counterpartId = table.Column<int>(type: "integer", nullable: false),
                    counterpartGroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_counterpartCounterpartGroup", x => x.counterpartId);
                    table.ForeignKey(
                        name: "FK_counterpartCounterpartGroup_counterpartGroup_counterpartGro~",
                        column: x => x.counterpartGroupId,
                        principalTable: "counterpartGroup",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_counterpartCounterpartGroup_counterpart_counterpartId",
                        column: x => x.counterpartId,
                        principalTable: "counterpart",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_counterpartCounterpartGroup_counterpartGroupId",
                table: "counterpartCounterpartGroup",
                column: "counterpartGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "counterpartCounterpartGroup");

            migrationBuilder.DropTable(
                name: "counterpartGroup");
        }
    }
}
