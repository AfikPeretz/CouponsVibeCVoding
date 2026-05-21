using CouponManager.Api.Enums;
using CouponManager.Api.Services.CouponParsing.Classifiers;

namespace CouponManager.Api.Tests.Classifiers;

public class CategoryClassifierTests
{
    [Theory]
    [InlineData("Shufersal",  null,    CouponCategory.Supermarket)]
    [InlineData("Golda",      null,    CouponCategory.Food)]
    [InlineData("All-InZone", null,    CouponCategory.MultiBrand)]
    [InlineData("Joe Delek",  null,    CouponCategory.FuelStationStore)]
    [InlineData(null,         "BuyMe", CouponCategory.GiftCard)]
    [InlineData(null,         null,    CouponCategory.Other)]
    [InlineData("Unknown",    "BuyMe", CouponCategory.Other)]   // known merchant beats provider rule
    public void Classify_ReturnsExpectedCategory(
        string? normalizedMerchant, string? provider, CouponCategory expected)
    {
        var result = CategoryClassifier.Classify(normalizedMerchant, provider);
        Assert.Equal(expected, result);
    }
}
