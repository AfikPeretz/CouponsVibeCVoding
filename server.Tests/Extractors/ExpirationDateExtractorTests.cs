using CouponManager.Api.Enums;
using CouponManager.Api.Services.CouponParsing.Extractors;

namespace CouponManager.Api.Tests.Extractors;

public class ExpirationDateExtractorTests
{
    [Fact]
    public void Extract_SlashDate_ReturnsExactDate()
    {
        var result = ExpirationDateExtractor.Extract("ותוקף השובר: 04/09/2026");

        Assert.Equal(ExpirationType.ExactDate, result.Type);
        Assert.Equal(new DateTime(2026, 9, 4, 0, 0, 0, DateTimeKind.Utc), result.Date);
    }

    [Fact]
    public void Extract_DotDate_TwoDigitYear_ReturnsExactDate()
    {
        var result = ExpirationDateExtractor.Extract("בתוקף עד ליום 31.05.26");

        Assert.Equal(ExpirationType.ExactDate, result.Type);
        Assert.Equal(new DateTime(2026, 5, 31, 0, 0, 0, DateTimeKind.Utc), result.Date);
    }

    [Fact]
    public void Extract_UntilNotice_ReturnsUntilNotice()
    {
        var result = ExpirationDateExtractor.Extract("השובר בתוקף עד הודעה חדשה.");

        Assert.Equal(ExpirationType.UntilNotice, result.Type);
        Assert.Null(result.Date);
    }

    [Fact]
    public void Extract_RelativeDuration_ReturnsRelativeDuration()
    {
        var result = ExpirationDateExtractor.Extract("תוקף השובר: 5 שנים");

        Assert.Equal(ExpirationType.RelativeDuration, result.Type);
        Assert.Null(result.Date);
        Assert.Contains("5 שנים", result.Text);
    }

    [Fact]
    public void Extract_NoExpiry_ReturnsUnknown()
    {
        var result = ExpirationDateExtractor.Extract("קופון ללא תאריך");

        Assert.Equal(ExpirationType.Unknown, result.Type);
        Assert.Null(result.Date);
        Assert.Null(result.Text);
    }
}
