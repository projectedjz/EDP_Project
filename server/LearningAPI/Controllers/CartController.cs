using LearningAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly MyDbContext _db;
        private readonly PromotionEngine _promoEngine;

        public CartController(MyDbContext db, PromotionEngine promoEngine)
        {
            _db = db;
            _promoEngine = promoEngine;
        }

        [HttpGet("{userId:int}/summary")]
        public async Task<IActionResult> GetSummary(int userId)
        {
            var cartLines = await _db.CartItems
                .Where(c => c.UserId == userId)
                .Join(_db.Products,
                    c => c.ProductId,
                    p => p.ProductId,
                    (c, p) => new PromotionEngine.CartLine(
                        p.ProductId,
                        c.CartQuantity,
                        p.Price
                    ))
                .ToListAsync();

            // then: load promo + call _promoEngine.Evaluate(...)
            return Ok(cartLines);
        }

        [HttpPost("{cartId:int}/apply-code")]
        public async Task<IActionResult> ApplyCode(int cartId, [FromBody] string code)
        {
            var cart = await _db.CartHeaders.FirstOrDefaultAsync(c => c.CartId == cartId);
            if (cart == null) return NotFound("Cart not found.");

            var promo = await _db.Promotions
                .Include(p => p.PromotionItems)
                .FirstOrDefaultAsync(p => p.RequiresCode && p.PromoCode == code);

            if (promo == null) return BadRequest("Invalid promo code.");

            var cartLines = await _db.CartItems
                .Where(ci => ci.CartItemId == cartId)
                .Join(_db.Products,
                    ci => ci.ProductId,
                    p => p.ProductId,
                    (ci, p) => new PromotionEngine.CartLine(p.ProductId, ci.CartQuantity, p.Price))
                .ToListAsync();

            var eval = _promoEngine.Evaluate(promo, cartLines, DateTime.UtcNow);
            if (!eval.IsEligible) return BadRequest(eval.Reason ?? "Promo not eligible.");

            if (cart.AppliedAutoPromotionId.HasValue)
            {
                var auto = await _db.Promotions.FirstAsync(p => p.PromotionId == cart.AppliedAutoPromotionId.Value);
                var canStack = auto.StackWithCode && promo.StackWithAuto;
                if (!canStack) return BadRequest("Cannot combine with current automatic promotion.");
            }

            cart.AppliedCodePromotionId = promo.PromotionId;
            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{cartId:int}/promo-code")]
        public async Task<IActionResult> RemoveCode(int cartId)
        {
            var cart = await _db.CartHeaders.FirstOrDefaultAsync(c => c.CartId == cartId);
            if (cart == null) return NotFound("Cart not found.");

            cart.AppliedCodePromotionId = null;
            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

    }

}
