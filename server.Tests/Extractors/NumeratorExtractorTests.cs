using CouponManager.Api.Services.CouponParsing.Extractors;

namespace CouponManager.Api.Tests.Extractors;

public class NumeratorExtractorTests
{
    [Theory]
    [InlineData("נומרטור: 131099865",  "131099865")]
    [InlineData("נומרטור:131099865",   "131099865")]
    [InlineData("נומרטור 131099865",   "131099865")]
    public void Extract_ReturnsNumerator(string text, string expected)
    {
        var result = NumeratorExtractor.Extract(text);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("אין נומרטור בהודעה")]
    [InlineData("")]
    public void Extract_ReturnsNull_WhenNotFound(string text)
    {
        var result = NumeratorExtractor.Extract(text);
        Assert.Null(result);
    }
}
