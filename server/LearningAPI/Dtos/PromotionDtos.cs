using System.ComponentModel.DataAnnotations;

namespace LearningAPI.Dtos;

public record PromotionItemUpsertDto(
    int? PromotionItemId,
    string? Role,          // "Qualifier" or "Target"
    int? RequiredQty,
    int? ProductId
);

public class PromotionCreateDto
{
    [MaxLength(30)]
    public string? PromoCode { get; set; }

    public bool RequiresCode { get; set; }

    [MaxLength(10)]
    public string? DiscountType { get; set; }

    public decimal? DiscountValue { get; set; }

    public bool IsExclusive { get; set; }
    public int? MinAmount { get; set; }
    public int? MinQuantity { get; set; }

    public DateTime? StartDatetime { get; set; }
    public DateTime? EndDatetime { get; set; }

    public bool IsActive { get; set; } = true;
    public int? CampaignId { get; set; }

    public List<PromotionItemUpsertDto> Items { get; set; } = new();
}

public class PromotionUpdateDto : PromotionCreateDto { }
