using CouponManager.Api.Enums;
using CouponManager.Api.Services.CouponParsing;

namespace CouponManager.Api.Tests;

/// <summary>
/// End-to-end tests for CouponParserService using fake coupon messages.
/// All text and URLs are fictional — no real coupon codes or private URLs.
/// </summary>
public class CouponParserIntegrationTests
{
    private readonly CouponParserService _parser = new();

    // -------------------------------------------------------------------------
    // Test 1: Shufersal / Pluxee
    // -------------------------------------------------------------------------
    private const string ShufersalText =
        "לצפיה בשובר שופרסל בסך ₪30.00: https://example.com/fake-voucher " +
        "לא ניתן לקבל עודף מהשובר. לא ניתן לממש באונליין. " +
        "השובר בתוקף עד הודעה חדשה.";

    [Fact]
    public void Parse_Shufersal_ExtractsAmount()
    {
        var result = _parser.Analyze(ShufersalText);
        Assert.Equal(30m, result.OriginalAmount);
    }

    [Fact]
    public void Parse_Shufersal_ExtractsMerchant()
    {
        var result = _parser.Analyze(ShufersalText);
        Assert.Equal("שופרסל", result.MerchantName);
    }

    [Fact]
    public void Parse_Shufersal_DetectsProvider()
    {
        var result = _parser.Analyze(ShufersalText);
        Assert.Equal("Pluxee", result.Provider);
    }

    [Fact]
    public void Parse_Shufersal_ExtractsVoucherUrl()
    {
        var result = _parser.Analyze(ShufersalText);
        Assert.Equal("https://example.com/fake-voucher", result.VoucherUrl);
    }

    [Fact]
    public void Parse_Shufersal_DetectsUntilNotice()
    {
        var result = _parser.Analyze(ShufersalText);
        Assert.Equal(ExpirationType.UntilNotice, result.ExpirationType);
    }

    [Fact]
    public void Parse_Shufersal_NotOnlineRedeemable()
    {
        var result = _parser.Analyze(ShufersalText);
        Assert.False(result.OnlineRedeemable);
    }

    // -------------------------------------------------------------------------
    // Test 2: BuyMe / Golda
    // -------------------------------------------------------------------------
    private const string GoldaText =
        "קיבלת מתנה ל-מארז קילו גלידה - באיסוף עצמי ב- גלידת גולדה - GOLDA! " +
        "קוד השובר: 1111-2222-3333-4444 " +
        "ותוקף השובר: 04/09/2026";

    [Fact]
    public void Parse_Golda_ExtractsMerchant()
    {
        var result = _parser.Analyze(GoldaText);
        Assert.Equal("Golda", result.NormalizedMerchantName);
    }

    [Fact]
    public void Parse_Golda_DetectsProvider()
    {
        var result = _parser.Analyze(GoldaText);
        Assert.Equal("BuyMe", result.Provider);
    }

    [Fact]
    public void Parse_Golda_ExtractsCouponCode()
    {
        var result = _parser.Analyze(GoldaText);
        Assert.Equal("1111-2222-3333-4444", result.CouponCode);
    }

    [Fact]
    public void Parse_Golda_ExtractsExpirationDate()
    {
        var result = _parser.Analyze(GoldaText);
        Assert.Equal(new DateTime(2026, 9, 4, 0, 0, 0, DateTimeKind.Utc), result.ExpirationDate);
    }

    [Fact]
    public void Parse_Golda_CategoryIsFood()
    {
        var result = _parser.Analyze(GoldaText);
        Assert.Equal(CouponCategory.Food, result.Category);
    }

    // -------------------------------------------------------------------------
    // Test 3: HTZone / All-InZone
    // -------------------------------------------------------------------------
    private const string AllInZoneText =
        "קיבלת שובר All-InZone בשווי 500 ₪ " +
        "הקוד שלך: 123456789-2003 " +
        "לרשימת הרשתות ובירור יתרה https://www.htzone.co.il/voucher-zone/10 " +
        "תוקף השובר: 5 שנים";

    [Fact]
    public void Parse_AllInZone_ExtractsMerchant()
    {
        var result = _parser.Analyze(AllInZoneText);
        Assert.Equal("All-InZone", result.MerchantName);
    }

    [Fact]
    public void Parse_AllInZone_DetectsProvider()
    {
        var result = _parser.Analyze(AllInZoneText);
        Assert.Equal("HTZone", result.Provider);
    }

    [Fact]
    public void Parse_AllInZone_ExtractsAmount()
    {
        var result = _parser.Analyze(AllInZoneText);
        Assert.Equal(500m, result.OriginalAmount);
    }

    [Fact]
    public void Parse_AllInZone_ExtractsCouponCode()
    {
        var result = _parser.Analyze(AllInZoneText);
        Assert.Equal("123456789-2003", result.CouponCode);
    }

    [Fact]
    public void Parse_AllInZone_ExpirationIsRelativeDuration()
    {
        var result = _parser.Analyze(AllInZoneText);
        Assert.Equal(ExpirationType.RelativeDuration, result.ExpirationType);
        Assert.Contains("5 שנים", result.ExpirationText);
    }

    [Fact]
    public void Parse_AllInZone_CategoryIsMultiBrand()
    {
        var result = _parser.Analyze(AllInZoneText);
        Assert.Equal(CouponCategory.MultiBrand, result.Category);
    }

    // -------------------------------------------------------------------------
    // Test 4: Delek UP / Joe Delek
    // -------------------------------------------------------------------------
    private const string JoDelekText =
        "קופון בסך 50 ₪ לרכישת מגוון מוצרי חלב בחנויות ג'ו דלק! " +
        "קוד הטבה: 655688 " +
        "נומרטור: 131099865 " +
        "בתוקף עד ליום 31.05.26";

    [Fact]
    public void Parse_JoDelek_ExtractsMerchant()
    {
        var result = _parser.Analyze(JoDelekText);
        Assert.Equal("ג'ו דלק", result.MerchantName);
    }

    [Fact]
    public void Parse_JoDelek_DetectsProvider()
    {
        var result = _parser.Analyze(JoDelekText);
        Assert.Equal("Delek UP", result.Provider);
    }

    [Fact]
    public void Parse_JoDelek_ExtractsAmount()
    {
        var result = _parser.Analyze(JoDelekText);
        Assert.Equal(50m, result.OriginalAmount);
    }

    [Fact]
    public void Parse_JoDelek_ExtractsCouponCode()
    {
        var result = _parser.Analyze(JoDelekText);
        Assert.Equal("655688", result.CouponCode);
    }

    [Fact]
    public void Parse_JoDelek_ExtractsNumerator()
    {
        var result = _parser.Analyze(JoDelekText);
        Assert.Equal("131099865", result.Numerator);
    }

    [Fact]
    public void Parse_JoDelek_ExtractsExpirationDate()
    {
        var result = _parser.Analyze(JoDelekText);
        Assert.Equal(new DateTime(2026, 5, 31, 0, 0, 0, DateTimeKind.Utc), result.ExpirationDate);
    }

    [Fact]
    public void Parse_JoDelek_CategoryIsFuelStationStore()
    {
        var result = _parser.Analyze(JoDelekText);
        Assert.Equal(CouponCategory.FuelStationStore, result.Category);
    }
}
