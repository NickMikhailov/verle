using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandGroupAndBrandGroupLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_brandBrandGroup_brandGroup_brandGroupId",
                table: "brandBrandGroup");

            migrationBuilder.AddForeignKey(
                name: "FK_brandBrandGroup_brandGroup_brandGroupId",
                table: "brandBrandGroup",
                column: "brandGroupId",
                principalTable: "brandGroup",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_brandBrandGroup_brandGroup_brandGroupId",
                table: "brandBrandGroup");

            migrationBuilder.AddForeignKey(
                name: "FK_brandBrandGroup_brandGroup_brandGroupId",
                table: "brandBrandGroup",
                column: "brandGroupId",
                principalTable: "brandGroup",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
