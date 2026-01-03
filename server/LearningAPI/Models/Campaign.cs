using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningAPI.Models
{
    public class Campaign
    {
        [Key]
        [Column("campaign_id")]
        public int CampaignId { get; set; }

        [Column("image_file")]
        [MaxLength(255)]
        public string? ImageFile { get; set; }

        [Column("status")]
        [MaxLength(20)]
        public string? Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
    }
}
