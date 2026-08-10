using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiSubmit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountCodeToCartItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountCodeId",
                table: "CarTItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAfterDiscount",
                table: "CarTItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_CarTItems_DiscountCodeId",
                table: "CarTItems",
                column: "DiscountCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarTItems_DiscountCodes_DiscountCodeId",
                table: "CarTItems",
                column: "DiscountCodeId",
                principalTable: "DiscountCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarTItems_DiscountCodes_DiscountCodeId",
                table: "CarTItems");

            migrationBuilder.DropIndex(
                name: "IX_CarTItems_DiscountCodeId",
                table: "CarTItems");

            migrationBuilder.DropColumn(
                name: "DiscountCodeId",
                table: "CarTItems");

            migrationBuilder.DropColumn(
                name: "PriceAfterDiscount",
                table: "CarTItems");
        }
    }
}
