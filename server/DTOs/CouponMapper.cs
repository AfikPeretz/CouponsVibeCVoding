using CouponManager.Api.Models;

namespace CouponManager.Api.DTOs;

internal static class CouponMapper
{
    internal static CouponDto ToDto(Coupon c) => new()
    {
        Id                     = c.Id,
        RawText                = c.RawText,
        Title                  = c.Title,
        Provider               = c.Provider,
        MerchantName           = c.MerchantName,
        NormalizedMerchantName = c.NormalizedMerchantName,
        Category               = c.Category,
        OriginalAmount         = c.OriginalAmount,
        RemainingAmount        = c.RemainingAmount,
        Currency               = c.Currency,
        CouponCode             = c.CouponCode,
        Numerator              = c.Numerator,
        VoucherUrl             = c.VoucherUrl,
        ExpirationDate         = c.ExpirationDate,
        ExpirationText         = c.ExpirationText,
        ExpirationType         = c.ExpirationType,
        OnlineRedeemable       = c.OnlineRedeemable,
        Status                 = c.Status,
        Confidence             = c.Confidence,
        ConditionsText         = c.ConditionsText,
        ReceivedAt             = c.ReceivedAt,
        CreatedAt              = c.CreatedAt,
        UpdatedAt              = c.UpdatedAt,
    };
}
