using LearningAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CartItem
{
    [Key]
    public int CartItemId { get; set; }
    public int CartQuantity { get; set; }

    public int? UserId { get; set; }
    public int ProductId { get; set; }
    public int SessionId { get; set; }

    // Navigation
    public User? User { get; set; }
    public Product? Product { get; set; }
    public GBLSession? Session { get; set; }


    [ForeignKey(nameof(CartItemId))]
    public CartHeader? CartHeader { get; set; }
}