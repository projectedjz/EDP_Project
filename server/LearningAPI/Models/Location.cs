using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LearningAPI.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }

        [Required, MaxLength(100)]
        public string LocationName { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Inventory>? Inventories { get; set; }
    }
}