using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LearningAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required, MaxLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? ProductImg { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Price { get; set; }

        // FK
        public int CategoryId { get; set; }

        // Navigation
        public Category? Category { get; set; }

        [JsonIgnore]
        public List<Inventory>? Inventories { get; set; }
    }
}