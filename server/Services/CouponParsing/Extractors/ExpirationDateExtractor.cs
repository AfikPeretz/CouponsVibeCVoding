using System.Text.RegularExpressions;
using CouponManager.Api.Enums;

namespace CouponManager.Api.Services.CouponParsing.Extractors;

public record ExpirationResult(
    DateTime? Date,
    string? Text,
    ExpirationType Type);

public static class ExpirationDateExtractor
{
    // "בתוקף עד הודעה חדשה" / "השובר בתוקף עד הודעה חדשה"
    private static readonly Regex _untilNotice =
        new(@"בתוקף עד הודעה חדשה", RegexOptions.Compiled);

    // DD/MM/YYYY  e.g. "04/09/2026"
    private static readonly Regex _slashDate =
        new(@"(\d{2})/(\d{2})/(\d{4})", RegexOptions.Compiled);

    // DD.MM.YY or DD.MM.YYYY  e.g. "31.05.26" or "31.05.2026"
    private static readonly Regex _dotDate =
        new(@"(\d{2})\.(\d{2})\.(\d{2,4})", RegexOptions.Compiled);

    // Relative duration: "תוקף ... : N שנים" or "תקף ל-N שנים" / "תקף ל N שנים"
    private static readonly Regex _relativeDuration = new(
        @"(?:תוקף[^:\n]*:|תקף\s*ל[-]?\s*)[^\n]*?(\d+\s*(?:שנים|חודשים|ימים|יום|שנה|חודש))",
        RegexOptions.Compiled);

    public static ExpirationResult Extract(string text)
    {
        // 1. Until further notice
        if (_untilNotice.IsMatch(text))
            return new ExpirationResult(null, "עד הודעה חדשה", ExpirationType.UntilNotice);

        // 2. Relative duration first — a text like "התקבל בתאריך 30/03/2025 ... תוקף 5 שנים"
        //    must not have the received date mis-classified as the expiration date.
        var relMatch = _relativeDuration.Match(text);
        if (relMatch.Success)
        {
            var durationText = relMatch.Groups[1].Value.Trim();
            return new ExpirationResult(null, durationText, ExpirationType.RelativeDuration);
        }

        // 3. Exact date — DD/MM/YYYY
        var slashMatch = _slashDate.Match(text);
        if (slashMatch.Success)
        {
            int day   = int.Parse(slashMatch.Groups[1].Value);
            int month = int.Parse(slashMatch.Groups[2].Value);
            int year  = int.Parse(slashMatch.Groups[3].Value);
            if (IsValidDate(day, month, year))
            {
                var date = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
                return new ExpirationResult(date, slashMatch.Value, ExpirationType.ExactDate);
            }
        }

        // 4. Exact date — DD.MM.YY or DD.MM.YYYY
        var dotMatch = _dotDate.Match(text);
        if (dotMatch.Success)
        {
            int day   = int.Parse(dotMatch.Groups[1].Value);
            int month = int.Parse(dotMatch.Groups[2].Value);
            int year  = int.Parse(dotMatch.Groups[3].Value);
            if (year < 100) year += 2000;   // 26 → 2026
            if (IsValidDate(day, month, year))
            {
                var date = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
                return new ExpirationResult(date, dotMatch.Value, ExpirationType.ExactDate);
            }
        }

        return new ExpirationResult(null, null, ExpirationType.Unknown);
    }

    private static bool IsValidDate(int day, int month, int year) =>
        month is >= 1 and <= 12 &&
        day is >= 1 and <= 31 &&
        year >= 2000;
}
