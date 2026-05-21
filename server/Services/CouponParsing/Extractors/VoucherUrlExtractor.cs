using System.Text.RegularExpressions;

namespace CouponManager.Api.Services.CouponParsing.Extractors;

public static class VoucherUrlExtractor
{
    private static readonly Regex _pattern =
        new(@"https?://[^\s]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static string? Extract(string text)
    {
        var match = _pattern.Match(text);
        return match.Success ? match.Value : null;
    }
}
