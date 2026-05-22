using System.Globalization;
using System.Text.RegularExpressions;

namespace CouponManager.Api.Services.CouponParsing.Extractors;

public static class RemainingAmountExtractor
{
    // Matches a remaining-balance phrase followed by a number (with optional ₪).
    // Hebrew phrases: "יתרה נוכחית", "יתרה", "נותרו", "נותר".
    // Longer phrase listed first so the regex engine prefers it.
    private static readonly Regex _pattern = new(
        @"(?:יתרה נוכחית|יתרה|נותרו|נותר)[:\s]+₪?\s*(\d+(?:\.\d+)?)\s*₪?",
        RegexOptions.Compiled);

    public static decimal? Extract(string text)
    {
        var match = _pattern.Match(text);
        if (match.Success &&
            decimal.TryParse(match.Groups[1].Value,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var amount))
        {
            return amount;
        }
        return null;
    }
}
