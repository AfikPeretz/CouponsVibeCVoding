using System.Text.RegularExpressions;

namespace CouponManager.Api.Services.CouponParsing.Extractors;

public static class ReceivedDateExtractor
{
    // "התקבל בתאריך 30/03/2025"  (DD/MM/YYYY)
    private static readonly Regex _slashFormat = new(
        @"התקבל\s*בתאריך\s*(\d{1,2})/(\d{1,2})/(\d{4})",
        RegexOptions.Compiled);

    // "התקבל ב30 למרץ 2025"  (DD <hebrew-month> YYYY, optional ל prefix on month)
    private static readonly Regex _hebrewMonthFormat = new(
        @"התקבל\s*ב\s*(\d{1,2})\s*ל?(ינואר|פברואר|מרץ|אפריל|מאי|יוני|יולי|אוגוסט|ספטמבר|אוקטובר|נובמבר|דצמבר)\s*(\d{4})",
        RegexOptions.Compiled);

    private static readonly Dictionary<string, int> _hebrewMonths = new()
    {
        ["ינואר"] = 1, ["פברואר"] = 2, ["מרץ"] = 3, ["אפריל"] = 4,
        ["מאי"] = 5, ["יוני"] = 6, ["יולי"] = 7, ["אוגוסט"] = 8,
        ["ספטמבר"] = 9, ["אוקטובר"] = 10, ["נובמבר"] = 11, ["דצמבר"] = 12,
    };

    public static DateTime? Extract(string text)
    {
        var slash = _slashFormat.Match(text);
        if (slash.Success)
        {
            int day = int.Parse(slash.Groups[1].Value);
            int month = int.Parse(slash.Groups[2].Value);
            int year = int.Parse(slash.Groups[3].Value);
            if (IsValidDate(day, month, year))
                return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        }

        var heb = _hebrewMonthFormat.Match(text);
        if (heb.Success && _hebrewMonths.TryGetValue(heb.Groups[2].Value, out var month2))
        {
            int day = int.Parse(heb.Groups[1].Value);
            int year = int.Parse(heb.Groups[3].Value);
            if (IsValidDate(day, month2, year))
                return new DateTime(year, month2, day, 0, 0, 0, DateTimeKind.Utc);
        }

        return null;
    }

    private static bool IsValidDate(int day, int month, int year) =>
        month is >= 1 and <= 12 &&
        day is >= 1 and <= 31 &&
        year >= 2000;
}
