using CouponManager.Api.Services.CouponParsing.Extractors;

namespace CouponManager.Api.Tests.Extractors;

public class RemainingAmountExtractorTests
{
    [Theory]
    [InlineData("יתרה 63.5",          63.5)]
    [InlineData("יתרה: 100",          100)]
    [InlineData("יתרה נוכחית 25.50",  25.50)]
    [InlineData("נותר ₪40",            40)]
    [InlineData("נותרו 12.5 ₪",        12.5)]
    public void Extract_ReturnsRemainingAmount(string text, decimal expected)
    {
        var result = RemainingAmountExtractor.Extract(text);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("שובר בשווי 500 ₪")]
    [InlineData("")]
    public void Extract_ReturnsNull_WhenNoRemainingPhrase(string text)
    {
        var result = RemainingAmountExtractor.Extract(text);
        Assert.Null(result);
    }
}
