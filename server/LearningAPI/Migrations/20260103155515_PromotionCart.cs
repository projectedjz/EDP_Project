using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningAPI.Migrations
{
    /// <inheritdoc />
    public partial class PromotionCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_CartHeaders_CartId",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "CartItems",
                newName: "CartItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_CartHeaders_CartItemId",
                table: "CartItems",
                column: "CartItemId",
                principalTable: "CartHeaders",
                principalColumn: "cart_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_CartHeaders_CartItemId",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "CartItemId",
                table: "CartItems",
                newName: "CartId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_CartHeaders_CartId",
                table: "CartItems",
                column: "CartId",
                principalTable: "CartHeaders",
                principalColumn: "cart_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
