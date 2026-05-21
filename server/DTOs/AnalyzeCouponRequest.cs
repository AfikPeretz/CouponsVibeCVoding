using System.ComponentModel.DataAnnotations;

namespace CouponManager.Api.DTOs;

public class AnalyzeCouponRequest
{
    [Required(ErrorMessage = "RawText is required.")]
    [MinLength(5, ErrorMessage = "RawText must be at least 5 characters.")]
    public string RawText { get; set; } = string.Empty;
}
