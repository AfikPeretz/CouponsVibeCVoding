using System.Text.RegularExpressions;

namespace CouponManager.Api.Services.CouponParsing.Extractors;

public static class NumeratorExtractor
{
    private static readonly Regex _pattern =
        new(@"נומרטור[:\s]+(\d+)", RegexOptions.Compiled);

    public static string? Extract(string text)
    {
        var match = _pattern.Match(text);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }
}
