namespace CouponManager.Api.Services.CouponParsing.Extractors;

public record MerchantResult(string? MerchantName, string? NormalizedMerchantName);

public static class MerchantExtractor
{
    public static MerchantResult Extract(string text)
    {
        // שופרסל (Shufersal) — various Hebrew forms and English spelling
        if (text.Contains("שופרסל") || text.Contains("Shufersal"))
            return new MerchantResult("שופרסל", "Shufersal");

        // גלידת גולדה (Golda ice cream) — Hebrew and English
        if (text.Contains("גלידת גולדה") || text.Contains("GOLDA") || text.Contains("Golda"))
            return new MerchantResult("גלידת גולדה", "Golda");

        // All-InZone (HTZone multi-brand voucher)
        if (text.Contains("All-InZone"))
            return new MerchantResult("All-InZone", "All-InZone");

        // ג'ו דלק (Joe Delek — Delek petrol station store)
        if (text.Contains("ג'ו דלק") || text.Contains("Joe Delek"))
            return new MerchantResult("ג'ו דלק", "Joe Delek");

        // Restaurants — last-resort fallback when no specific merchant matched
        // and the text indicates a restaurants voucher.
        if (text.Contains("מסעדות"))
            return new MerchantResult("מסעדות", "Restaurants");

        return new MerchantResult(null, null);
    }
}
