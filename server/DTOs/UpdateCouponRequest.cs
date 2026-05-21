namespace CouponManager.Api.DTOs;

public class UpdateCouponRequest
{
    public string? Title { get; set; }
    public string? Provider { get; set; }
    public string? MerchantName { get; set; }
    public string? NormalizedMerchantName { get; set; }
    public int Category { get; set; }
    public decimal? OriginalAmount { get; set; }
    public decimal? RemainingAmount { get; set; }
    public string? Currency { get; set; }
    public string? CouponCode { get; set; }
    public string? Numerator { get; set; }
    public string? VoucherUrl { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? ExpirationText { get; set; }
    public int ExpirationType { get; set; }
    public bool OnlineRedeemable { get; set; }
    public int Status { get; set; }
    public string? ConditionsText { get; set; }
}
