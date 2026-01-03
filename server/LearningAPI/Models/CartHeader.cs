using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningAPI.Models;

public class CartHeader
{
    [Key]
    [Column("cart_id")]
    public int CartId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("applied_auto_promotion_id")]
    public int? AppliedAutoPromotionId { get; set; }

    [Column("applied_code_promotion_id")]
    public int? AppliedCodePromotionId { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
