using CouponManager.Api.Services.CouponParsing.Extractors;

namespace CouponManager.Api.Tests.Extractors;

public class MerchantExtractorTests
{
    [Theory]
    [InlineData("שובר שופרסל בסך ₪30",        "שופרסל",      "Shufersal")]
    [InlineData("שופרסל אקסטרא",               "שופרסל",      "Shufersal")]
    [InlineData("ב- גלידת גולדה - GOLDA!",     "גלידת גולדה", "Golda")]
    [InlineData("גלידה ב-Golda",               "גלידת גולדה", "Golda")]
    [InlineData("שובר All-InZone בשווי 500",   "All-InZone",  "All-InZone")]
    [InlineData("בחנויות ג'ו דלק",             "ג'ו דלק",     "Joe Delek")]
    public void Extract_ReturnsCorrectMerchant(string text,
        string expectedMerchant, string expectedNormalized)
    {
        var result = MerchantExtractor.Extract(text);
        Assert.Equal(expectedMerchant, result.MerchantName);
        Assert.Equal(expectedNormalized, result.NormalizedMerchantName);
    }

    [Fact]
    public void Extract_ReturnsNull_WhenMerchantNotFound()
    {
        var result = MerchantExtractor.Extract("קופון ללא זיהוי מוכר");
        Assert.Null(result.MerchantName);
        Assert.Null(result.NormalizedMerchantName);
    }
}
