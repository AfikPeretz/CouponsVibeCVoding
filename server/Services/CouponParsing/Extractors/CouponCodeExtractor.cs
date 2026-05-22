using System.Text.RegularExpressions;

namespace CouponManager.Api.Services.CouponParsing.Extractors;

public static class CouponCodeExtractor
{
    // Looks for a known label, then captures the code on the same or next line.
    // Supported labels: "קוד השובר שלך", "קוד השובר", "הקוד שלך", "קוד הטבה", "מספר שובר".
    // Longer phrases listed first so the regex engine prefers them.
    // Supported code formats:
    //   - Dashed:  1111-2222-3333-4444 / 123456789-2003 / 118283629-2124
    //   - Numeric: 655688 / 20573525
    private static readonly Regex _pattern = new(
        @"(?:קוד השובר שלך|קוד השובר|הקוד שלך|קוד הטבה|מספר שובר)[:\s]+([\d][\d\-]*)",
        RegexOptions.Compiled);

    public static string? Extract(string text)
    {
        var match = _pattern.Match(text);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }
}
