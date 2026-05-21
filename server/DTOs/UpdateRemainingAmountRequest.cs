using System.ComponentModel.DataAnnotations;

namespace CouponManager.Api.DTOs;

public class UpdateRemainingAmountRequest
{
    [Range(0, double.MaxValue, ErrorMessage = "remainingAmount must be >= 0")]
    public decimal RemainingAmount { get; set; }
}
