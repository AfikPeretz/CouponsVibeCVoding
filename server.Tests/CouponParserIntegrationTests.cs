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

    // -------------------------------------------------------------------------
    // Test 5: All-InZone with יתרה / קוד השובר שלך / received-date + 5 שנים
    // -------------------------------------------------------------------------
    private const string AllInZoneRemainingText =
        "התקבל בתאריך 30/03/2025 שובר All-InZone בשווי 510 ₪. " +
        "יתרה 63.5. " +
        "קוד השובר שלך: 118283629-2124. " +
        "תוקף השובר: 5 שנים";

    [Fact]
    public void Parse_AllInZoneRemaining_OriginalAmount()
    {
        var result = _parser.Analyze(AllInZoneRemainingText);
        Assert.Equal(510m, result.OriginalAmount);
    }

    [Fact]
    public void Parse_AllInZoneRemaining_RemainingAmount()
    {
        var result = _parser.Analyze(AllInZoneRemainingText);
        Assert.Equal(63.5m, result.RemainingAmount);
    }

    [Fact]
    public void Parse_AllInZoneRemaining_CouponCode()
    {
        var result = _parser.Analyze(AllInZoneRemainingText);
        Assert.Equal("118283629-2124", result.CouponCode);
    }

    [Fact]
    public void Parse_AllInZoneRemaining_ProviderAndMerchant()
    {
        var result = _parser.Analyze(AllInZoneRemainingText);
        Assert.Equal("HTZone", result.Provider);
        Assert.Equal("All-InZone", result.MerchantName);
    }

    [Fact]
    public void Parse_AllInZoneRemaining_CategoryIsMultiBrand()
    {
        var result = _parser.Analyze(AllInZoneRemainingText);
        Assert.Equal(CouponCategory.MultiBrand, result.Category);
    }

    [Fact]
    public void Parse_AllInZoneRemaining_RelativeDurationProjectedFromReceivedDate()
    {
        var result = _parser.Analyze(AllInZoneRemainingText);
        Assert.Equal(ExpirationType.RelativeDuration, result.ExpirationType);
        Assert.Contains("5 שנים", result.ExpirationText);
        Assert.Equal(new DateTime(2030, 3, 30, 0, 0, 0, DateTimeKind.Utc), result.ExpirationDate);
    }

    // -------------------------------------------------------------------------
    // Test 6: Restaurant voucher with מספר שובר
    // -------------------------------------------------------------------------
    private const string RestaurantText =
        "שובר למסעדות שף בסך 150 ₪. " +
        "מספר שובר: 20573525";

    [Fact]
    public void Parse_Restaurant_Amount()
    {
        var result = _parser.Analyze(RestaurantText);
        Assert.Equal(150m, result.OriginalAmount);
    }

    [Fact]
    public void Parse_Restaurant_MerchantFallback()
    {
        var result = _parser.Analyze(RestaurantText);
        Assert.Equal("מסעדות", result.MerchantName);
        Assert.Equal("Restaurants", result.NormalizedMerchantName);
    }

    [Fact]
    public void Parse_Restaurant_CategoryIsFood()
    {
        var result = _parser.Analyze(RestaurantText);
        Assert.Equal(CouponCategory.Food, result.Category);
    }

    [Fact]
    public void Parse_Restaurant_CouponCode()
    {
        var result = _parser.Analyze(RestaurantText);
        Assert.Equal("20573525", result.CouponCode);
    }
}
