using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CouponManager.Api.Data;
using CouponManager.Api.DTOs;
using CouponManager.Api.Enums;
using CouponManager.Api.Models;
using CouponManager.Api.Services.CouponParsing;

namespace CouponManager.Api.Controllers;

[ApiController]
[Route("api/coupons")]
public class CouponsController : ControllerBase
{
    private readonly ICouponParserService _parser;
    private readonly AppDbContext _db;

    public CouponsController(ICouponParserService parser, AppDbContext db)
    {
        _parser = parser;
        _db = db;
    }

    // ── POST /api/coupons/analyze ────────────────────────────────────────────
    /// <summary>Analyzes raw coupon text. Does not save to the database.</summary>
    [HttpPost("analyze")]
    [ProducesResponseType(typeof(CouponAnalysisResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Analyze([FromBody] AnalyzeCouponRequest request)
    {
        var result = _parser.Analyze(request.RawText);
        return Ok(result);
    }

    // ── POST /api/coupons ────────────────────────────────────────────────────
    /// <summary>Saves a confirmed coupon to the database.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateCouponRequest request)
    {
        var code = string.IsNullOrWhiteSpace(request.CouponCode) ? null : request.CouponCode;
        var url  = string.IsNullOrWhiteSpace(request.VoucherUrl) ? null : request.VoucherUrl;
        var checkRaw = code is null && url is null;

        var isDuplicate = await _db.Coupons.AnyAsync(c =>
            (code != null && c.CouponCode == code) ||
            (url  != null && c.VoucherUrl == url) ||
            (checkRaw && c.RawText == request.RawText));

        if (isDuplicate)
            return Conflict(new { message = "שובר זה כבר קיים במערכת" });

        var now = DateTime.UtcNow;

        var coupon = new Coupon
        {
            RawText                = request.RawText,
            Title                  = string.IsNullOrWhiteSpace(request.Title)    ? "קופון"    : request.Title,
            Provider               = request.Provider,
            MerchantName           = string.IsNullOrWhiteSpace(request.MerchantName) ? "לא זוהה" : request.MerchantName,
            NormalizedMerchantName = request.NormalizedMerchantName,
            Category               = (CouponCategory)request.Category,
            OriginalAmount         = request.OriginalAmount,
            RemainingAmount        = request.RemainingAmount,
            Currency               = string.IsNullOrWhiteSpace(request.Currency) ? "ILS"      : request.Currency,
            CouponCode             = request.CouponCode,
            Numerator              = request.Numerator,
            VoucherUrl             = request.VoucherUrl,
            ExpirationDate         = request.ExpirationDate,
            ExpirationText         = request.ExpirationText,
            ExpirationType         = (ExpirationType)request.ExpirationType,
            OnlineRedeemable       = request.OnlineRedeemable,
            Status                 = request.Status == 0 ? CouponStatus.Active : (CouponStatus)request.Status,
            Confidence             = request.Confidence,
            ConditionsText         = request.ConditionsText,
            ReceivedAt             = now,
            CreatedAt              = now,
            UpdatedAt              = now,
        };

        _db.Coupons.Add(coupon);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = coupon.Id }, CouponMapper.ToDto(coupon));
    }

    // ── GET /api/coupons ─────────────────────────────────────────────────────
    /// <summary>Returns all coupons: active first, then by expiry, then by creation date.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CouponDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var coupons = await _db.Coupons
            .OrderBy(c => c.Status == CouponStatus.Active ? 0 : 1)
            .ThenBy(c => c.ExpirationDate.HasValue ? 0 : 1)
            .ThenBy(c => c.ExpirationDate)
            .ThenByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Ok(coupons.Select(CouponMapper.ToDto));
    }

    // ── GET /api/coupons/search?query= ───────────────────────────────────────
    /// <summary>
    /// Searches coupons by title, merchantName, normalizedMerchantName, provider, or couponCode.
    /// Returns an empty list when query is blank.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<CouponDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Ok(Array.Empty<CouponDto>());

        var q = query.Trim();

        var coupons = await _db.Coupons
            .Where(c =>
                (c.Title != null && c.Title.Contains(q)) ||
                (c.MerchantName != null && c.MerchantName.Contains(q)) ||
                (c.NormalizedMerchantName != null && c.NormalizedMerchantName.Contains(q)) ||
                (c.Provider != null && c.Provider.Contains(q)) ||
                (c.CouponCode != null && c.CouponCode.Contains(q)))
            .OrderBy(c => c.Status == CouponStatus.Active ? 0 : 1)
            .ThenBy(c => c.ExpirationDate.HasValue ? 0 : 1)
            .ThenBy(c => c.ExpirationDate)
            .ThenByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Ok(coupons.Select(CouponMapper.ToDto));
    }

    // ── GET /api/coupons/{id} ────────────────────────────────────────────────
    /// <summary>Returns a single coupon by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var coupon = await _db.Coupons.FindAsync(id);
        if (coupon is null) return NotFound();
        return Ok(CouponMapper.ToDto(coupon));
    }

    // ── PUT /api/coupons/{id} ────────────────────────────────────────────────
    /// <summary>Updates all editable fields of a coupon.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCouponRequest request)
    {
        var coupon = await _db.Coupons.FindAsync(id);
        if (coupon is null) return NotFound();

        coupon.Title                  = string.IsNullOrWhiteSpace(request.Title)        ? "קופון"    : request.Title;
        coupon.Provider               = request.Provider;
        coupon.MerchantName           = string.IsNullOrWhiteSpace(request.MerchantName) ? "לא זוהה" : request.MerchantName;
        coupon.NormalizedMerchantName = request.NormalizedMerchantName;
        coupon.Category               = (CouponCategory)request.Category;
        coupon.OriginalAmount         = request.OriginalAmount;
        coupon.RemainingAmount        = request.RemainingAmount;
        coupon.Currency               = string.IsNullOrWhiteSpace(request.Currency)     ? "ILS"      : request.Currency;
        coupon.CouponCode             = request.CouponCode;
        coupon.Numerator              = request.Numerator;
        coupon.VoucherUrl             = request.VoucherUrl;
        coupon.ExpirationDate         = request.ExpirationDate;
        coupon.ExpirationText         = request.ExpirationText;
        coupon.ExpirationType         = (ExpirationType)request.ExpirationType;
        coupon.OnlineRedeemable       = request.OnlineRedeemable;
        coupon.Status                 = (CouponStatus)request.Status;
        coupon.ConditionsText         = request.ConditionsText;
        coupon.UpdatedAt              = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(CouponMapper.ToDto(coupon));
    }

    // ── PATCH /api/coupons/{id}/remaining-amount ─────────────────────────────
    /// <summary>Updates only the remaining amount.</summary>
    [HttpPatch("{id:int}/remaining-amount")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRemainingAmount(int id, [FromBody] UpdateRemainingAmountRequest request)
    {
        var coupon = await _db.Coupons.FindAsync(id);
        if (coupon is null) return NotFound();

        coupon.RemainingAmount = request.RemainingAmount;
        coupon.UpdatedAt       = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(CouponMapper.ToDto(coupon));
    }

    // ── PATCH /api/coupons/{id}/mark-used ────────────────────────────────────
    /// <summary>Sets status to Used and remaining amount to 0 if it was set.</summary>
    [HttpPatch("{id:int}/mark-used")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkUsed(int id)
    {
        var coupon = await _db.Coupons.FindAsync(id);
        if (coupon is null) return NotFound();

        coupon.Status    = CouponStatus.Used;
        if (coupon.RemainingAmount.HasValue)
            coupon.RemainingAmount = 0;
        coupon.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(CouponMapper.ToDto(coupon));
    }

    // ── PATCH /api/coupons/{id}/archive ──────────────────────────────────────
    /// <summary>Sets status to Archived.</summary>
    [HttpPatch("{id:int}/archive")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Archive(int id)
    {
        var coupon = await _db.Coupons.FindAsync(id);
        if (coupon is null) return NotFound();

        coupon.Status    = CouponStatus.Archived;
        coupon.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(CouponMapper.ToDto(coupon));
    }

    // ── DELETE /api/coupons/{id} ──────────────────────────────────────────────
    /// <summary>Permanently deletes a coupon.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var coupon = await _db.Coupons.FindAsync(id);
        if (coupon is null) return NotFound();

        _db.Coupons.Remove(coupon);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
