using System.ComponentModel.DataAnnotations;

namespace CouponManager.Api.DTOs;

public class CreateCouponRequest
{
    [Required(ErrorMessage = "RawText is required.")]
    public string RawText { get; set; } = string.Empty;

    public string? Title { get; set; }
    public string? Provider { get; set; }
    public string? MerchantName { get; set; }
    public string? NormalizedMerchantName { get; set; }

    // Numeric enum value from the frontend (CouponCategory)
    public int Category { get; set; }

    public decimal? OriginalAmount { get; set; }
    public decimal? RemainingAmount { get; set; }
    public string? Currency { get; set; }
    public string? CouponCode { get; set; }
    public string? Numerator { get; set; }
    public string? VoucherUrl { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? ExpirationText { get; set; }

    // Numeric enum value from the frontend (ExpirationType)
    public int ExpirationType { get; set; }

    public bool OnlineRedeemable { get; set; }

    // Numeric enum value from the frontend (CouponStatus)
    public int Status { get; set; }

    public double Confidence { get; set; }
    public string? ConditionsText { get; set; }
}
