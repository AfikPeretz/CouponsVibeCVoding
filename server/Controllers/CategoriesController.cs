using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CouponManager.Api.Data;
using CouponManager.Api.DTOs;
using CouponManager.Api.Enums;
using CouponManager.Api.Services;

namespace CouponManager.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private static readonly Dictionary<CouponCategory, string> DisplayNames = new()
    {
        [CouponCategory.Unknown]          = "לא ידוע",
        [CouponCategory.Food]             = "מזון",
        [CouponCategory.Fashion]          = "אופנה",
        [CouponCategory.Travel]           = "נסיעות",
        [CouponCategory.Health]           = "בריאות",
        [CouponCategory.Entertainment]    = "בידור",
        [CouponCategory.Supermarket]      = "סופרמרקט",
        [CouponCategory.MultiBrand]       = "רב-מותגי",
        [CouponCategory.FuelStationStore] = "תחנת דלק",
        [CouponCategory.GiftCard]         = "כרטיס מתנה",
        [CouponCategory.Other]            = "אחר",
    };

    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db) => _db = db;

    // ── GET /api/categories ──────────────────────────────────────────────────
    /// <summary>
    /// Returns all categories that have at least one coupon,
    /// with total count, active count, and total active remaining amount.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategorySummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories()
    {
        var coupons = await _db.Coupons.ToListAsync();

        var summaries = coupons
            .GroupBy(c => c.Category)
            .Select(g =>
            {
                var active = g.Where(CouponExpirationHelper.IsEffectivelyActive).ToList();

                decimal? totalRemaining = active.Any(c => c.RemainingAmount.HasValue)
                    ? active.Where(c => c.RemainingAmount.HasValue).Sum(c => c.RemainingAmount!.Value)
                    : null;

                return new CategorySummaryDto
                {
                    Category                   = (int)g.Key,
                    DisplayName                = DisplayNames.GetValueOrDefault(g.Key, g.Key.ToString()),
                    TotalCoupons               = g.Count(),
                    ActiveCoupons              = active.Count,
                    TotalActiveRemainingAmount = totalRemaining,
                };
            })
            .OrderBy(s => s.Category)
            .ToList();

        return Ok(summaries);
    }

    // ── GET /api/categories/{category}/coupons ───────────────────────────────
    /// <summary>
    /// Returns all coupons in a given category.
    /// Effectively-active coupons appear first, then sorted by expiration date ascending.
    /// </summary>
    [HttpGet("{category:int}/coupons")]
    [ProducesResponseType(typeof(IEnumerable<CouponDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCouponsByCategory(int category)
    {
        var coupons = await _db.Coupons
            .Where(c => (int)c.Category == category)
            .ToListAsync();

        var sorted = coupons
            .OrderBy(c => CouponExpirationHelper.IsEffectivelyActive(c) ? 0 : 1)
            .ThenBy(c => c.ExpirationDate.HasValue ? 0 : 1)
            .ThenBy(c => c.ExpirationDate)
            .ThenByDescending(c => c.CreatedAt)
            .Select(CouponMapper.ToDto)
            .ToList();

        return Ok(sorted);
    }
}
