using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesUniqueImportKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UX_sales_counterpartId1C_document_nomenclaturaId1C",
                table: "sales",
                columns: new[] { "counterpartId1C", "document", "nomenclaturaId1C" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_sales_counterpartId1C_document_nomenclaturaId1C",
                table: "sales");
        }
    }
}
