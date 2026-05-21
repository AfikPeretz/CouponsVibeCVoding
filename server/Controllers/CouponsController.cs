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
    public async Task<IActionResult> Create([FromBody] CreateCouponRequest request)
    {
        var now = DateTime.UtcNow;

        var coupon = new Coupon
        {
            RawText                = request.RawText,
            Title                  = string.IsNullOrWhiteSpace(request.Title)
                                        ? "קופון" : request.Title,
            Provider               = request.Provider,
            MerchantName           = string.IsNullOrWhiteSpace(request.MerchantName)
                                        ? "לא זוהה" : request.MerchantName,
            NormalizedMerchantName = request.NormalizedMerchantName,
            Category               = (CouponCategory)request.Category,
            OriginalAmount         = request.OriginalAmount,
            RemainingAmount        = request.RemainingAmount,
            Currency               = string.IsNullOrWhiteSpace(request.Currency)
                                        ? "ILS" : request.Currency,
            CouponCode             = request.CouponCode,
            Numerator              = request.Numerator,
            VoucherUrl             = request.VoucherUrl,
            ExpirationDate         = request.ExpirationDate,
            ExpirationText         = request.ExpirationText,
            ExpirationType         = (ExpirationType)request.ExpirationType,
            OnlineRedeemable       = request.OnlineRedeemable,
            Status                 = request.Status == 0
                                        ? CouponStatus.Active : (CouponStatus)request.Status,
            Confidence             = request.Confidence,
            ConditionsText         = request.ConditionsText,
            ReceivedAt             = now,
            CreatedAt              = now,
            UpdatedAt              = now,
        };

        _db.Coupons.Add(coupon);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = coupon.Id }, MapToDto(coupon));
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

        return Ok(coupons.Select(MapToDto));
    }

    // ── GET /api/coupons/{id} ────────────────────────────────────────────────
    /// <summary>Returns a single coupon by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var coupon = await _db.Coupons.FindAsync(id);
        if (coupon is null)
            return NotFound();

        return Ok(MapToDto(coupon));
    }

    // ── Private mapper ───────────────────────────────────────────────────────
    private static CouponDto MapToDto(Coupon c) => new()
    {
        Id                     = c.Id,
        RawText                = c.RawText,
        Title                  = c.Title,
        Provider               = c.Provider,
        MerchantName           = c.MerchantName,
        NormalizedMerchantName = c.NormalizedMerchantName,
        Category               = c.Category,
        OriginalAmount         = c.OriginalAmount,
        RemainingAmount        = c.RemainingAmount,
        Currency               = c.Currency,
        CouponCode             = c.CouponCode,
        Numerator              = c.Numerator,
        VoucherUrl             = c.VoucherUrl,
        ExpirationDate         = c.ExpirationDate,
        ExpirationText         = c.ExpirationText,
        ExpirationType         = c.ExpirationType,
        OnlineRedeemable       = c.OnlineRedeemable,
        Status                 = c.Status,
        Confidence             = c.Confidence,
        ConditionsText         = c.ConditionsText,
        ReceivedAt             = c.ReceivedAt,
        CreatedAt              = c.CreatedAt,
        UpdatedAt              = c.UpdatedAt,
    };
}
