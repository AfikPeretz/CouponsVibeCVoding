namespace CouponManager.Api.Enums;

public enum ExpirationType
{
    Unknown = 0,
    ExactDate = 1,          // Expires on a specific calendar date
    UntilNotice = 2,        // Valid until further notice
    RelativeDuration = 3    // e.g. "5 years", "30 days from issue"
}
