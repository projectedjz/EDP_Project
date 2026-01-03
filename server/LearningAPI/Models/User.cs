using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LearningAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100), JsonIgnore]
        public string Password { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // 1–1 Navigation
        public Staff? Staff { get; set; }
        public Customer? Customer { get; set; }

        // 1–0..1 Navigation
        public Cart? Cart { get; set; }

        // 1–many Navigation
        public List<Order>? Orders { get; set; }
        public List<GBLSession>? GBLSessions { get; set; }

        [JsonIgnore]
        public List<Tutorial>? Tutorials { get; set; }
    }
}