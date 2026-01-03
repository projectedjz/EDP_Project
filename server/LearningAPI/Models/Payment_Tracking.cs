using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningAPI.Models
{
    public class PaymentTracking
    {
        [Key]
        public int PaymentId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime PaidBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime ExpiresOn { get; set; }

        [MaxLength(50)]
        public string PaymentStatus { get; set; } = string.Empty;

        // FKs
        public int OrderId { get; set; }
        public int SessionId { get; set; }

        // Navigation
        public Order? Order { get; set; }
        public GBLSession? Session { get; set; }
    }
}