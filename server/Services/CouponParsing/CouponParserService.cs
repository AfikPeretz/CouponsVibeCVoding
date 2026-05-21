using CouponManager.Api.DTOs;
using CouponManager.Api.Enums;
using CouponManager.Api.Services.CouponParsing.Classifiers;
using CouponManager.Api.Services.CouponParsing.Confidence;
using CouponManager.Api.Services.CouponParsing.Extractors;

namespace CouponManager.Api.Services.CouponParsing;

public class CouponParserService : ICouponParserService
{
    public CouponAnalysisResultDto Analyze(string rawText)
    {
        var amount     = AmountExtractor.Extract(rawText);
        var url        = VoucherUrlExtractor.Extract(rawText);
        var code       = CouponCodeExtractor.Extract(rawText);
        var numerator  = NumeratorExtractor.Extract(rawText);
        var expiry     = ExpirationDateExtractor.Extract(rawText);
        var provider   = ProviderExtractor.Extract(rawText);
        var merchant   = MerchantExtractor.Extract(rawText);
        var category   = CategoryClassifier.Classify(merchant.NormalizedMerchantName, provider);
        var conditions = ConditionExtractor.Extract(rawText);

        var result = new CouponAnalysisResultDto
        {
            Title                  = GenerateTitle(merchant.MerchantName, amount),
            Provider               = provider,
            MerchantName           = merchant.MerchantName,
            NormalizedMerchantName = merchant.NormalizedMerchantName,
            Category               = category,
            OriginalAmount         = amount,
            RemainingAmount        = amount,    // same as original until partial-use is tracked
            Currency               = "ILS",
            CouponCode             = code,
            Numerator              = numerator,
            VoucherUrl             = url,
            ExpirationDate         = expiry.Date,
            ExpirationText         = expiry.Text,
            ExpirationType         = expiry.Type,
            OnlineRedeemable       = conditions.OnlineRedeemable ?? true,
            Status                 = CouponStatus.Active,
            ConditionsText         = conditions.ConditionsText,
            Warnings               = []
        };

        result.Confidence = CouponConfidenceCalculator.Calculate(result);

        // Warn about missing critical fields
        if (!amount.HasValue)
            result.Warnings.Add("Amount was not found");
        if (string.IsNullOrEmpty(merchant.MerchantName))
            result.Warnings.Add("Merchant was not found");
        if (expiry.Type == ExpirationType.Unknown)
            result.Warnings.Add("Expiration date was not found");

        return result;
    }

    private static string GenerateTitle(string? merchantName, decimal? amount)
    {
        if (!string.IsNullOrEmpty(merchantName) && amount.HasValue)
            return $"קופון {merchantName} {amount:0.##} ₪";
        if (!string.IsNullOrEmpty(merchantName))
            return $"קופון {merchantName}";
        if (amount.HasValue)
            return $"קופון {amount:0.##} ₪";
        return "קופון";
    }
}
