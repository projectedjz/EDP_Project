using System.ComponentModel.DataAnnotations;

namespace LearningAPI.Models
{
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }

        // FK
        public int UserId { get; set; }

        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Role { get; set; } = string.Empty;

        // Navigation
        public User? User { get; set; }
    }
}