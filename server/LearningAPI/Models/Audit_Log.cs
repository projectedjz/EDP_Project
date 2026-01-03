using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningAPI.Models
{
    public class AuditLog
    {
        [Key]
        public int AuditId { get; set; }

        [MaxLength(20)]
        public string Action { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50)]
        public string AffectedTable { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        // FK
        public int UserId { get; set; }

        public User? User { get; set; }
    }
}