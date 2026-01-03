using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LearningAPI.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, MaxLength(20)]
        public string CategoryName { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Product>? Products { get; set; }
    }
}