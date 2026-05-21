using CouponManager.Api.Services.CouponParsing.Extractors;

namespace CouponManager.Api.Tests.Extractors;

public class VoucherUrlExtractorTests
{
    [Theory]
    [InlineData("לצפיה בשובר: https://example.com/fake-voucher סיום",
                "https://example.com/fake-voucher")]
    [InlineData("https://www.htzone.co.il/voucher-zone/10 ועוד טקסט",
                "https://www.htzone.co.il/voucher-zone/10")]
    [InlineData("http://example.com/code123",
                "http://example.com/code123")]
    public void Extract_ReturnsFirstUrl(string text, string expected)
    {
        var result = VoucherUrlExtractor.Extract(text);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("אין קישור בהודעה")]
    [InlineData("")]
    public void Extract_ReturnsNull_WhenNoUrl(string text)
    {
        var result = VoucherUrlExtractor.Extract(text);
        Assert.Null(result);
    }
}
