namespace CouponManager.Api.Services.CouponParsing.Extractors;

public record ConditionResult(string? ConditionsText, bool? OnlineRedeemable);

public static class ConditionExtractor
{
    private static readonly string[] _knownConditions =
    [
        "לא ניתן לקבל עודף",
        "ללא כפל מבצעים",
        "כולל כפל מבצעים"
    ];

    public static ConditionResult Extract(string text)
    {
        var found = _knownConditions.Where(text.Contains).ToList();

        bool? onlineRedeemable = null;
        if (text.Contains("לא ניתן לממש באונליין"))
        {
            onlineRedeemable = false;
            found.Add("לא ניתן לממש באונליין");
        }
        else if (text.Contains("ניתן לממש באונליין"))
        {
            onlineRedeemable = true;
        }

        var conditionsText = found.Count > 0 ? string.Join(". ", found) : null;
        return new ConditionResult(conditionsText, onlineRedeemable);
    }
}
