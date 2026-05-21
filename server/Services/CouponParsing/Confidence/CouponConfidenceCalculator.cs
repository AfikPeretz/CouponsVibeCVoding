using CouponManager.Api.DTOs;
using CouponManager.Api.Enums;

namespace CouponManager.Api.Services.CouponParsing.Confidence;

public static class CouponConfidenceCalculator
{
    public static double Calculate(CouponAnalysisResultDto result)
    {
        double score = 0.0;

        if (result.OriginalAmount.HasValue)             score += 0.25;
        if (!string.IsNullOrEmpty(result.MerchantName)) score += 0.25;
        if (!string.IsNullOrEmpty(result.Provider))     score += 0.15;
        if (result.ExpirationType != ExpirationType.Unknown) score += 0.20;
        if (!string.IsNullOrEmpty(result.VoucherUrl))   score += 0.10;
        if (!string.IsNullOrEmpty(result.CouponCode))   score += 0.05;

        return Math.Round(score, 2);
    }
}
