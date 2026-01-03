using Mysqlx.Crud;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningAPI.Models
{
    // Models/Promotion.cs
    public class Promotion
    {
        [Key]
        [Column("promotion_id")]
        public int PromotionId { get; set; }

        [Column("promo_code")]
        [MaxLength(30)]
        public string? PromoCode { get; set; }

        [Column("requires_code")]
        [Required]
        public bool RequiresCode { get; set; }

        [Column("discount_type")]
        [MaxLength(10)]
        public string? DiscountType { get; set; }

        [Column("discount_value", TypeName = "decimal(10,2)")]
        public decimal? DiscountValue { get; set; }

        [Column("is_exclusive")]
        public bool IsExclusive { get; set; } = false;

        [Column("min_amount")]
        public int? MinAmount { get; set; }

        [Column("min_quantity")]
        public int? MinQuantity { get; set; }

        [Column("usage_count")]
        public int UsageCount { get; set; } = 0;

        [Column("start_datetime")]
        public DateTime? StartDatetime { get; set; }

        [Column("end_datetime")]
        public DateTime? EndDatetime { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("campaign_id")]
        public int? CampaignId { get; set; }

        // Navigation properties
        [ForeignKey("CampaignId")]
        public Campaign? Campaign { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<PromotionItem> PromotionItems { get; set; } = new List<PromotionItem>();
    }
}