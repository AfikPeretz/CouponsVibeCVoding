using System.Globalization;
using System.Text.RegularExpressions;

namespace CouponManager.Api.Services.CouponParsing.Extractors;

public static class AmountExtractor
{
    // Matches ₪ before the number: ₪30.00  ₪ 200
    private static readonly Regex _currencyFirst =
        new(@"₪\s*(\d+(?:\.\d+)?)", RegexOptions.Compiled);

    // Matches ₪ after the number: 500 ₪  50₪
    private static readonly Regex _currencyLast =
        new(@"(\d+(?:\.\d+)?)\s*₪", RegexOptions.Compiled);

    public static decimal? Extract(string text)
    {
        var match = _currencyFirst.Match(text);
        if (!match.Success)
            match = _currencyLast.Match(text);

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
