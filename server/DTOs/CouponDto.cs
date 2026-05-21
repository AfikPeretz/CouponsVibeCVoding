using CouponManager.Api.Enums;

namespace CouponManager.Api.DTOs;

public class CouponDto
{
    public int Id { get; set; }
    public string RawText { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Provider { get; set; }
    public string? MerchantName { get; set; }
    public string? NormalizedMerchantName { get; set; }
    public CouponCategory Category { get; set; }
    public decimal? OriginalAmount { get; set; }
    public decimal? RemainingAmount { get; set; }
    public string? Currency { get; set; }
    public string? CouponCode { get; set; }
    public string? Numerator { get; set; }
    public string? VoucherUrl { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? ExpirationText { get; set; }
    public ExpirationType ExpirationType { get; set; }
    public bool OnlineRedeemable { get; set; }
    public CouponStatus Status { get; set; }
    public double Confidence { get; set; }
    public string? ConditionsText { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
