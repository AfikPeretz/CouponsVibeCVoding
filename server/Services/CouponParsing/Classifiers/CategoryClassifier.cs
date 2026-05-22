using CouponManager.Api.Enums;

namespace CouponManager.Api.Services.CouponParsing.Classifiers;

public static class CategoryClassifier
{
    public static CouponCategory Classify(string? normalizedMerchant, string? provider)
    {
        return normalizedMerchant switch
        {
            "Shufersal"   => CouponCategory.Supermarket,
            "Golda"       => CouponCategory.Food,
            "All-InZone"  => CouponCategory.MultiBrand,
            "Joe Delek"   => CouponCategory.FuelStationStore,
            "Restaurants" => CouponCategory.Food,
            // BuyMe voucher with no identified merchant → generic gift card
            null when provider == "BuyMe" => CouponCategory.GiftCard,
            _ => CouponCategory.Other
        };
    }
}
