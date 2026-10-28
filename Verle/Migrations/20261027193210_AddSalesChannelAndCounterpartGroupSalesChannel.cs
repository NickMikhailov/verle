using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesChannelAndCounterpartGroupSalesChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_counterpartCounterpartGroup_counterpartGroup_counterpartGro~",
                table: "counterpartCounterpartGroup");

            migrationBuilder.CreateTable(
                name: "salesChannel",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salesChannel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "counterpartGroupSalesChannel",
                columns: table => new
                {
                    counterpartGroupId = table.Column<int>(type: "integer", nullable: false),
                    salesChannelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_counterpartGroupSalesChannel", x => x.counterpartGroupId);
                    table.ForeignKey(
                        name: "FK_counterpartGroupSalesChannel_counterpartGroup_counterpartGr~",
                        column: x => x.counterpartGroupId,
                        principalTable: "counterpartGroup",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_counterpartGroupSalesChannel_salesChannel_salesChannelId",
                        column: x => x.salesChannelId,
                        principalTable: "salesChannel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_counterpartGroupSalesChannel_salesChannelId",
                table: "counterpartGroupSalesChannel",
                column: "salesChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_counterpartCounterpartGroup_counterpartGroup_counterpartGro~",
                table: "counterpartCounterpartGroup",
                column: "counterpartGroupId",
                principalTable: "counterpartGroup",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_counterpartCounterpartGroup_counterpartGroup_counterpartGro~",
                table: "counterpartCounterpartGroup");

            migrationBuilder.DropTable(
                name: "counterpartGroupSalesChannel");

            migrationBuilder.DropTable(
                name: "salesChannel");

            migrationBuilder.AddForeignKey(
                name: "FK_counterpartCounterpartGroup_counterpartGroup_counterpartGro~",
                table: "counterpartCounterpartGroup",
                column: "counterpartGroupId",
                principalTable: "counterpartGroup",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
