using CouponManager.Api.Enums;

namespace CouponManager.Api.Models;

public class Coupon
{
    public int Id { get; set; }

    // The original raw SMS/message text pasted by the user
    public string RawText { get; set; } = string.Empty;

    // Human-readable title, either extracted or user-edited
    public string? Title { get; set; }

    // Who issued the coupon (e.g. "Pluxee", "Multipass")
    public string? Provider { get; set; }

    // Merchant name as found in the raw text
    public string? MerchantName { get; set; }

    // Canonical merchant name after normalization
    public string? NormalizedMerchantName { get; set; }

    public CouponCategory Category { get; set; } = CouponCategory.Unknown;

    // Face value at the time of issue
    public decimal? OriginalAmount { get; set; }

    // Remaining usable value (may differ after partial redemption)
    public decimal? RemainingAmount { get; set; }

    // Currency code, e.g. "ILS", "USD"
    public string? Currency { get; set; }

    // Alphanumeric redemption code
    public string? CouponCode { get; set; }

    // "1 of 3 uses" style text
    public string? Numerator { get; set; }

    // Redemption URL (fake/placeholder in dev)
    public string? VoucherUrl { get; set; }

    // Parsed expiration date
    public DateTime? ExpirationDate { get; set; }

    // Raw expiration string from the message (e.g. "עד 31.12.25")
    public string? ExpirationText { get; set; }

    public ExpirationType ExpirationType { get; set; } = ExpirationType.Unknown;

    // Whether the coupon can be used online
    public bool OnlineRedeemable { get; set; }

    public CouponStatus Status { get; set; } = CouponStatus.Active;

    // Parser confidence score (0.0–1.0)
    public double Confidence { get; set; }

    // Raw conditions paragraph from the message
    public string? ConditionsText { get; set; }

    // When the original message was reportedly sent/received
    public DateTime? ReceivedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
