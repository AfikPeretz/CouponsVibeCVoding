using CouponManager.Api.DTOs;

namespace CouponManager.Api.Services.CouponParsing;

public interface ICouponParserService
{
    /// <summary>
    /// Analyzes raw coupon text and returns structured coupon data.
    /// </summary>
    CouponAnalysisResultDto Analyze(string rawText);
}
