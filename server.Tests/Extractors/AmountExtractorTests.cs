using CouponManager.Api.Services.CouponParsing.Extractors;

namespace CouponManager.Api.Tests.Extractors;

public class AmountExtractorTests
{
    [Theory]
    [InlineData("בסך ₪30.00",  30.00)]
    [InlineData("בסך ₪200.00", 200.00)]
    [InlineData("בשווי 500 ₪", 500)]
    [InlineData("בסך 50 ₪",    50)]
    [InlineData("סכום: ₪200.00", 200.00)]
    public void Extract_ReturnsCorrectAmount(string text, decimal expected)
    {
        var result = AmountExtractor.Extract(text);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("אין מחיר בהודעה זו")]
    [InlineData("")]
    public void Extract_ReturnsNull_WhenNoAmountFound(string text)
    {
        var result = AmountExtractor.Extract(text);
        Assert.Null(result);
    }
}
