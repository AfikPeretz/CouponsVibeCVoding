using CouponManager.Api.Services.CouponParsing.Extractors;

namespace CouponManager.Api.Tests.Extractors;

public class ReceivedDateExtractorTests
{
    [Fact]
    public void Extract_SlashFormat_ReturnsDate()
    {
        var result = ReceivedDateExtractor.Extract("התקבל בתאריך 30/03/2025");
        Assert.Equal(new DateTime(2025, 3, 30, 0, 0, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void Extract_HebrewMonthFormat_ReturnsDate()
    {
        var result = ReceivedDateExtractor.Extract("התקבל ב30 למרץ 2025");
        Assert.Equal(new DateTime(2025, 3, 30, 0, 0, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void Extract_HebrewMonthFormat_WithoutLamedPrefix_ReturnsDate()
    {
        var result = ReceivedDateExtractor.Extract("התקבל ב15 ינואר 2024");
        Assert.Equal(new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc), result);
    }

    [Theory]
    [InlineData("אין תאריך התקבלות")]
    [InlineData("")]
    public void Extract_ReturnsNull_WhenNoReceivedDate(string text)
    {
        var result = ReceivedDateExtractor.Extract(text);
        Assert.Null(result);
    }
}
