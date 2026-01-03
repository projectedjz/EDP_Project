using LearningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningAPI.Services;

public sealed class PromotionEngine
{
    // Keep this in sync with your DB values (Promotion.discount_type).
    private static readonly HashSet<string> PercentTypes =
        new(StringComparer.OrdinalIgnoreCase) { "PERCENT", "PERCENTAGE" };

    private static readonly HashSet<string> AmountTypes =
        new(StringComparer.OrdinalIgnoreCase) { "AMOUNT", "FIXED" };

    public sealed record CartLine(int ProductId, int Quantity, decimal UnitPrice);

    public sealed record PromotionEvaluationResult(
        bool IsEligible,
        string? Reason,
        int EligibleUnits,
        int DiscountedUnits,
        decimal DiscountBaseSubtotal,
        decimal DiscountAmount
    );

    /// <summary>
    /// Checks: IsActive + date window + min_amount + min_quantity + qualifier required_qty + has target items (if targets exist).
    /// If eligible, calculates discount using promo.DiscountType/DiscountValue and caps units using promo.MaxQuantity.
    /// </summary>
    public PromotionEvaluationResult Evaluate(Promotion promo, IReadOnlyList<CartLine> cartLines, DateTime nowUtc)
    {
        // 1) Active + date window
        if (!promo.IsActive)
            return new(false, "Promotion inactive.", 0, 0, 0m, 0m);

        if (promo.StartDatetime.HasValue && nowUtc < promo.StartDatetime.Value)
            return new(false, "Promotion not started.", 0, 0, 0m, 0m);

        if (promo.EndDatetime.HasValue && nowUtc > promo.EndDatetime.Value)
            return new(false, "Promotion ended.", 0, 0, 0m, 0m);

        // 2) Cart-level thresholds
        var cartSubtotal = cartLines.Sum(l => l.UnitPrice * l.Quantity);
        var cartQty = cartLines.Sum(l => l.Quantity);

        if (promo.MinAmount.HasValue && cartSubtotal < promo.MinAmount.Value)
            return new(false, "Min amount not met.", 0, 0, 0m, 0m);

        if (promo.MinQuantity.HasValue && cartQty < promo.MinQuantity.Value)
            return new(false, "Min quantity not met.", 0, 0, 0m, 0m);

        // 3) Qualifier checks (required_qty per qualifier product)
        var qualifiers = promo.PromotionItems
            .Where(i => i.Role != null && i.Role.Equals("Qualifier", StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var q in qualifiers)
        {
            var required = q.RequiredQty ?? 0;
            var inCartQty = cartLines.Where(l => l.ProductId == q.ProductId).Sum(l => l.Quantity);

            if (inCartQty < required)
                return new(false, $"Qualifier product {q.ProductId} quantity not met.", 0, 0, 0m, 0m);
        }

        // 4) Decide which lines are discountable (targets preferred; else qualifiers)
        var targetIds = promo.PromotionItems
            .Where(i => i.Role != null && i.Role.Equals("Target", StringComparison.OrdinalIgnoreCase))
            .Select(i => i.ProductId)
            .ToHashSet();

        List<CartLine> discountableLines;

        if (targetIds.Count > 0)
        {
            discountableLines = cartLines.Where(l => targetIds.Contains(l.ProductId) && l.Quantity > 0).ToList();
            if (discountableLines.Count == 0)
                return new(false, "No target items in cart.", 0, 0, 0m, 0m);
        }
        else
        {
            // If your promo always has targets, you can change this to "return not eligible".
            var qualifierIds = qualifiers.Select(q => q.ProductId).ToHashSet();
            discountableLines = cartLines.Where(l => qualifierIds.Contains(l.ProductId) && l.Quantity > 0).ToList();
            if (discountableLines.Count == 0)
                return new(false, "No discountable items in cart.", 0, 0, 0m, 0m);
        }

        var eligibleUnits = discountableLines.Sum(l => l.Quantity);

        var discountedUnits = eligibleUnits;
        if (promo.MaxQuantity.HasValue)
            discountedUnits = Math.Min(eligibleUnits, promo.MaxQuantity.Value);

        // Base subtotal to discount: pick highest priced units first (prevents “prorating” inaccuracies)
        var baseSubtotal = SumTopUnitsSubtotal(discountableLines, discountedUnits);

        // 5) Apply discount_type/value
        var type = (promo.DiscountType ?? "").Trim();
        var value = promo.DiscountValue ?? 0m;

        decimal discountAmount = 0m;

        if (PercentTypes.Contains(type))
        {
            discountAmount = baseSubtotal * (value / 100m);
        }
        else if (AmountTypes.Contains(type))
        {
            // Fixed amount off the base subtotal (cap so it never goes negative)
            discountAmount = value;
        }
        else
        {
            return new(false, $"Unsupported discount_type '{promo.DiscountType}'.", eligibleUnits, 0, baseSubtotal, 0m);
        }

        // Cap discount so it never exceeds the subtotal it applies to
        discountAmount = Math.Min(discountAmount, baseSubtotal);
        discountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero);

        return new(true, null, eligibleUnits, discountedUnits, baseSubtotal, discountAmount);
    }

    /// <summary>
    /// Atomic redemption: increments usage_count only if usage_limit_total not reached.
    /// Call this at checkout finalization, not when user is just “viewing cart”.
    /// </summary>
    public async Task<bool> TryRedeemAsync(MyDbContext db, int promotionId, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        // IMPORTANT: table/column names must match your MySQL schema.
        var rows = await db.Database.ExecuteSqlRawAsync(@"
            UPDATE Promotion
            SET usage_count = usage_count + 1
            WHERE promotion_id = {0}
              AND (usage_limit_total IS NULL OR usage_count < usage_limit_total)
        ", [promotionId], ct);

        if (rows == 0)
        {
            await tx.RollbackAsync(ct);
            return false; // limit reached
        }

        await tx.CommitAsync(ct);
        return true;
    }

    private static decimal SumTopUnitsSubtotal(IEnumerable<CartLine> lines, int unitsToTake)
    {
        if (unitsToTake <= 0) return 0m;

        var ordered = lines.OrderByDescending(l => l.UnitPrice);

        var remaining = unitsToTake;
        decimal subtotal = 0m;

        foreach (var line in ordered)
        {
            if (remaining <= 0) break;

            var take = Math.Min(remaining, line.Quantity);
            subtotal += take * line.UnitPrice;
            remaining -= take;
        }

        return subtotal;
    }
}
