using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningAPI.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal TotalPrice { get; set; }

        [MaxLength(30)]
        public string OrderStatus { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        // FKs
        public int UserId { get; set; }
        public int SessionId { get; set; }

        // Navigation
        public User? User { get; set; }
        public GBLSession? Session { get; set; }
    }
}