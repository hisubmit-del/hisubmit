using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiSubmit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNullableToDiscountCodeToCartItemTB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarTItems_DiscountCodes_DiscountCodeId",
                table: "CarTItems");

            migrationBuilder.AlterColumn<int>(
                name: "DiscountCodeId",
                table: "CarTItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CarTItems_DiscountCodes_DiscountCodeId",
                table: "CarTItems",
                column: "DiscountCodeId",
                principalTable: "DiscountCodes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarTItems_DiscountCodes_DiscountCodeId",
                table: "CarTItems");

            migrationBuilder.AlterColumn<int>(
                name: "DiscountCodeId",
                table: "CarTItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CarTItems_DiscountCodes_DiscountCodeId",
                table: "CarTItems",
                column: "DiscountCodeId",
                principalTable: "DiscountCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
