using LearningAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearningAPI.Dtos;

namespace LearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PromotionsController : ControllerBase
    {
        private readonly MyDbContext _db;

        public PromotionsController(MyDbContext db)
        {
            _db = db;
        }

        // READ all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promotion>>> GetAll()
        {
            var promos = await _db.Promotions
                .Include(p => p.PromotionItems)
                .ToListAsync();

            return Ok(promos);
        }

        // READ one
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Promotion>> GetById(int id)
        {
            var promo = await _db.Promotions
                .Include(p => p.PromotionItems)
                .FirstOrDefaultAsync(p => p.PromotionId == id);

            return promo is null ? NotFound() : Ok(promo);
        }

        // CREATE
        [HttpPost]
        public async Task<ActionResult<Promotion>> Create([FromBody] PromotionCreateDto dto)
        {
            ValidatePromoItems(dto.Items);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            if (dto.CampaignId.HasValue)
            {
                var campaignOk = await _db.Campaigns.AnyAsync(c => c.CampaignId == dto.CampaignId.Value);
                if (!campaignOk) return BadRequest("Invalid campaignId.");
            }

            // Optional: validate products exist
            var productIds = dto.Items.Where(i => i.ProductId.HasValue).Select(i => i.ProductId!.Value).Distinct().ToList();
            var existingProductCount = await _db.Products.CountAsync(p => productIds.Contains(p.ProductId));
            if (existingProductCount != productIds.Count) return BadRequest("One or more productId is invalid.");

            var promo = new Promotion
            {
                PromoCode = dto.PromoCode,
                RequiresCode = dto.RequiresCode,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                IsExclusive = dto.IsExclusive,
                MinAmount = dto.MinAmount,
                MinQuantity = dto.MinQuantity,
                UsageCount = 0,
                StartDatetime = dto.StartDatetime,
                EndDatetime = dto.EndDatetime,
                IsActive = dto.IsActive,
                CampaignId = dto.CampaignId,
                PromotionItems = dto.Items.Select(i => new PromotionItem
                {
                    Role = i.Role,
                    RequiredQty = i.RequiredQty,
                    ProductId = i.ProductId
                }).ToList()
            };

            _db.Promotions.Add(promo);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = promo.PromotionId }, promo);
        }

        // UPDATE (simple: replace fields + replace items)
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Promotion>> Update(int id, [FromBody] PromotionUpdateDto dto)
        {
            ValidatePromoItems(dto.Items);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var promo = await _db.Promotions
                .Include(p => p.PromotionItems)
                .FirstOrDefaultAsync(p => p.PromotionId == id);

            if (promo is null) return NotFound();

            promo.PromoCode = dto.PromoCode;
            promo.RequiresCode = dto.RequiresCode;
            promo.DiscountType = dto.DiscountType;
            promo.DiscountValue = dto.DiscountValue;
            promo.IsExclusive = dto.IsExclusive;
            promo.MinAmount = dto.MinAmount;
            promo.MinQuantity = dto.MinQuantity;
            promo.StartDatetime = dto.StartDatetime;
            promo.EndDatetime = dto.EndDatetime;
            promo.IsActive = dto.IsActive;
            promo.CampaignId = dto.CampaignId;

            // replace child rows
            _db.PromotionItems.RemoveRange(promo.PromotionItems);
            promo.PromotionItems = dto.Items.Select(i => new PromotionItem
            {
                Role = i.Role,
                RequiredQty = i.RequiredQty,
                ProductId = i.ProductId,
                PromotionId = promo.PromotionId
            }).ToList();

            await _db.SaveChangesAsync();
            return Ok(promo);
        }

        // DELETE
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var promo = await _db.Promotions
                .Include(p => p.PromotionItems)
                .FirstOrDefaultAsync(p => p.PromotionId == id);

            if (promo is null) return NotFound();

            _db.PromotionItems.RemoveRange(promo.PromotionItems);
            _db.Promotions.Remove(promo);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // Business rule: Qualifiers required; targets only allowed if qualifiers exist.
        private void ValidatePromoItems(List<PromotionItemUpsertDto> items)
        {
            items ??= new();

            var qualifierCount = items.Count(i =>
                string.Equals(i.Role, "Qualifier", StringComparison.OrdinalIgnoreCase));

            var targetCount = items.Count(i =>
                string.Equals(i.Role, "Target", StringComparison.OrdinalIgnoreCase));

            if (qualifierCount == 0)
                ModelState.AddModelError("items", "At least 1 Qualifier is required.");

            if (targetCount > 0 && qualifierCount == 0)
                ModelState.AddModelError("items", "Targets are not allowed without Qualifiers.");

            if (items.Any(i => i.ProductId is null))
                ModelState.AddModelError("items", "Each PromotionItem must have a productId.");

            if (items.Any(i => i.Role is null ||
                (!string.Equals(i.Role, "Qualifier", StringComparison.OrdinalIgnoreCase) &&
                 !string.Equals(i.Role, "Target", StringComparison.OrdinalIgnoreCase))))
                ModelState.AddModelError("items", "Role must be 'Qualifier' or 'Target'.");
        }
    }
}
