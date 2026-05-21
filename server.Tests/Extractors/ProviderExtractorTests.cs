using CouponManager.Api.Services.CouponParsing.Extractors;

namespace CouponManager.Api.Tests.Extractors;

public class ProviderExtractorTests
{
    [Theory]
    [InlineData("לצפיה בשובר שופרסל בסך ₪30.00: https://example.com/fake", "Pluxee")]
    [InlineData("קופון pluxee לשופרסל", "Pluxee")]
    [InlineData("קיבלת מתנה ל-מארז גלידה ב- גלידת גולדה!", "BuyMe")]
    [InlineData("קיבלת שובר All-InZone בשווי 500 ₪ https://www.htzone.co.il/x", "HTZone")]
    [InlineData("כרטיסיה buyme לרשת", "BuyMe")]
    [InlineData("קופון בסך 50 ₪ לחנויות ג'ו דלק!", "Delek UP")]
    [InlineData("קופון אפליקציית דלק UP", "Delek UP")]
    public void Extract_ReturnsCorrectProvider(string text, string expected)
    {
        var result = ProviderExtractor.Extract(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Extract_ReturnsNull_WhenNoProviderFound()
    {
        var result = ProviderExtractor.Extract("קופון ללא ספק מזוהה");
        Assert.Null(result);
    }
}
