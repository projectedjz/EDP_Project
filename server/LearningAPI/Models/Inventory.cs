using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningAPI.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime HarvestDate { get; set; }


        [Column(TypeName = "datetime")]
        public DateTime ExpiryDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        // FKs
        public int ProductId { get; set; }
        public int LocationId { get; set; }

        // Navigation
        public Product? Product { get; set; }
        public Location? Location { get; set; }
    }
}