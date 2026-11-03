using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Verle.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSalesQuantityToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_counterpartCounterpartGroup_counterpartGroup_counterpartGro~",
                table: "counterpartCounterpartGroup");

            migrationBuilder.AlterColumn<double>(
                name: "quantity",
                table: "sales",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "nomenclaturaClass",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_counterpartCounterpartGroup_counterpartGroup_counterpartGro~",
                table: "counterpartCounterpartGroup",
                column: "counterpartGroupId",
                principalTable: "counterpartGroup",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_counterpartCounterpartGroup_counterpartGroup_counterpartGro~",
                table: "counterpartCounterpartGroup");

            migrationBuilder.AlterColumn<int>(
                name: "quantity",
                table: "sales",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "nomenclaturaClass",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddForeignKey(
                name: "FK_counterpartCounterpartGroup_counterpartGroup_counterpartGro~",
                table: "counterpartCounterpartGroup",
                column: "counterpartGroupId",
                principalTable: "counterpartGroup",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
