using CouponManager.Api.Services.CouponParsing.Extractors;

namespace CouponManager.Api.Tests.Extractors;

public class ConditionExtractorTests
{
    [Fact]
    public void Extract_NotOnline_ReturnsFalseAndConditions()
    {
        const string text = "לא ניתן לקבל עודף מהשובר. לא ניתן לממש באונליין.";

        var result = ConditionExtractor.Extract(text);

        Assert.False(result.OnlineRedeemable);
        Assert.NotNull(result.ConditionsText);
        Assert.Contains("לא ניתן לקבל עודף", result.ConditionsText);
        Assert.Contains("לא ניתן לממש באונליין", result.ConditionsText);
    }

    [Fact]
    public void Extract_NoConditions_ReturnsNulls()
    {
        var result = ConditionExtractor.Extract("קופון רגיל ללא תנאים");

        Assert.Null(result.OnlineRedeemable);
        Assert.Null(result.ConditionsText);
    }

    [Fact]
    public void Extract_LloCumulativePromotions_IncludesInConditions()
    {
        var result = ConditionExtractor.Extract("ללא כפל מבצעים");

        Assert.NotNull(result.ConditionsText);
        Assert.Contains("ללא כפל מבצעים", result.ConditionsText);
    }
}
