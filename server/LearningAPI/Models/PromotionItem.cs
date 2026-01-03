using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningAPI.Models
{
    // Models/PromotionItem.cs
    public class PromotionItem
    {
        [Key]
        [Column("promotion_item_id")]
        public int PromotionItemId { get; set; }

        [Column("role")]
        [MaxLength(20)]
        public string? Role { get; set; }

        [Column("required_qty")]
        public int? RequiredQty { get; set; }

        [Column("promotion_id")]
        public int? PromotionId { get; set; }

        [Column("product_id")]
        public int? ProductId { get; set; }

        // Navigation properties
        [ForeignKey("PromotionId")]
        public Promotion? Promotion { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}
