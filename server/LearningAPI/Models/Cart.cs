using LearningAPI.Models;
using System.ComponentModel.DataAnnotations;

public class Cart
{
    [Key]
    public int CartId { get; set; }
    public int CartQuantity { get; set; }

    public int? UserId { get; set; }
    public int ProductId { get; set; }
    public int SessionId { get; set; }

    // Navigation
    public User? User { get; set; }
    public Product? Product { get; set; }
    public GBLSession? Session { get; set; }
}