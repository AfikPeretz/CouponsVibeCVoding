namespace CouponManager.Api.Services.CouponParsing.Extractors;

public static class ProviderExtractor
{
    public static string? Extract(string text)
    {
        var lower = text.ToLowerInvariant();

        // Pluxee — keyword in text/URL, or the phrase "לצפיה בשובר" (Pluxee's SMS format)
        if (lower.Contains("pluxee") || text.Contains("לצפיה בשובר"))
            return "Pluxee";

        // Cibus — check after Pluxee to avoid matching "cibus.pluxee"
        if (lower.Contains("cibus"))
            return "Cibus";

        // BuyMe — keyword, or the phrase "קיבלת מתנה" (BuyMe's SMS format)
        if (lower.Contains("buyme") || text.Contains("קיבלת מתנה"))
            return "BuyMe";

        // HTZone — keyword in text/URL, or brand name "All-InZone"
        if (lower.Contains("htzone") || text.Contains("All-InZone"))
            return "HTZone";

        // Delek UP — keyword, or ג'ו דלק (Joe Delek is a Delek-chain store)
        if (text.Contains("דלק UP") || text.Contains("אפליקציית דלק UP") || text.Contains("ג'ו דלק"))
            return "Delek UP";

        return null;
    }
}
