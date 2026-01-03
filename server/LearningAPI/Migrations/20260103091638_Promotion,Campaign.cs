using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace LearningAPI.Migrations
{
    /// <inheritdoc />
    public partial class PromotionCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PromotionId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    campaign_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    image_file = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.campaign_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    promotion_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    promo_code = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    requires_code = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    discount_type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    discount_value = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    is_exclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    min_amount = table.Column<int>(type: "int", nullable: true),
                    min_quantity = table.Column<int>(type: "int", nullable: true),
                    usage_count = table.Column<int>(type: "int", nullable: false),
                    start_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    end_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    campaign_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.promotion_id);
                    table.ForeignKey(
                        name: "FK_Promotions_Campaigns_campaign_id",
                        column: x => x.campaign_id,
                        principalTable: "Campaigns",
                        principalColumn: "campaign_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PromotionItems",
                columns: table => new
                {
                    promotion_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    role = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    required_qty = table.Column<int>(type: "int", nullable: true),
                    promotion_id = table.Column<int>(type: "int", nullable: true),
                    product_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionItems", x => x.promotion_item_id);
                    table.ForeignKey(
                        name: "FK_PromotionItems_Products_product_id",
                        column: x => x.product_id,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_PromotionItems_Promotions_promotion_id",
                        column: x => x.promotion_id,
                        principalTable: "Promotions",
                        principalColumn: "promotion_id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PromotionId",
                table: "Orders",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionItems_product_id",
                table: "PromotionItems",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionItems_promotion_id",
                table: "PromotionItems",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_campaign_id",
                table: "Promotions",
                column: "campaign_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Promotions_PromotionId",
                table: "Orders",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "promotion_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Promotions_PromotionId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "PromotionItems");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PromotionId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "Orders");
        }
    }
}
