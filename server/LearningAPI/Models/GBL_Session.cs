using Mysqlx.Crud;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LearningAPI.Models
{
    public class GBLSession
    {
        [Key]
        public int SessionId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        // FKs
        public int UserId { get; set; }
        public int? CartId { get; set; }

        // Navigation
        public User? User { get; set; }
        public CartItem? CartItems { get; set; }

        [JsonIgnore]
        public List<Order>? Orders { get; set; }

        [JsonIgnore]
        public List<PaymentTracking>? Payments { get; set; }
    }
}