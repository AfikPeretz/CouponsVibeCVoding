namespace CouponManager.Api.DTOs;

public class CategorySummaryDto
{
    public int Category { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int TotalCoupons { get; set; }
    public int ActiveCoupons { get; set; }
    public decimal? TotalActiveRemainingAmount { get; set; }
}
