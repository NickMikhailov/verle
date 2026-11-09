using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionOrderExecutionJournal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "executionStatus",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_executionStatus", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "productionOrderExecutionEvent",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    orderId = table.Column<int>(type: "integer", nullable: false),
                    occurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    executionStatusId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productionOrderExecutionEvent", x => x.id);
                    table.ForeignKey(
                        name: "FK_productionOrderExecutionEvent_executionStatus_executionStat~",
                        column: x => x.executionStatusId,
                        principalTable: "executionStatus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productionOrderExecutionEvent_productionOrder_orderId",
                        column: x => x.orderId,
                        principalTable: "productionOrder",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "executionStatus",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { 1, "InProgress", "В работе" },
                    { 2, "Downtime", "Простой" },
                    { 3, "Finished", "Выполнение завершено" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_executionStatus_code",
                table: "executionStatus",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_executionEvent_orderId_id",
                table: "productionOrderExecutionEvent",
                columns: new[] { "orderId", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_productionOrderExecutionEvent_executionStatusId",
                table: "productionOrderExecutionEvent",
                column: "executionStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "productionOrderExecutionEvent");

            migrationBuilder.DropTable(
                name: "executionStatus");
        }
    }
}
