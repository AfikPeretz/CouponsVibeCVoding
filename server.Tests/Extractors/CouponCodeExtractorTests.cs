using CouponManager.Api.Services.CouponParsing.Extractors;

namespace CouponManager.Api.Tests.Extractors;

public class CouponCodeExtractorTests
{
    [Theory]
    [InlineData("קוד השובר: 1111-2222-3333-4444 ועוד",    "1111-2222-3333-4444")]
    [InlineData("הקוד שלך: 123456789-2003 לרשימה",         "123456789-2003")]
    [InlineData("קוד הטבה: 655688 נומרטור",                "655688")]
    public void Extract_ReturnsCode(string text, string expected)
    {
        var result = CouponCodeExtractor.Extract(text);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("אין קוד בהודעה זו")]
    [InlineData("")]
    public void Extract_ReturnsNull_WhenNoCode(string text)
    {
        var result = CouponCodeExtractor.Extract(text);
        Assert.Null(result);
    }
}
