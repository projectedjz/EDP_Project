using System.ComponentModel.DataAnnotations;

namespace LearningAPI.Models
{
    public class Customer
    {
        [Key]
        public int CustId { get; set; }

        // FK
        public int UserId { get; set; }

        [MaxLength(30)]
        public string CustType { get; set; } = string.Empty;

        // Navigation
        public User? User { get; set; }
    }
}