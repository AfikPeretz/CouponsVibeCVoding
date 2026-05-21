using CouponManager.Api.Enums;
using CouponManager.Api.Models;

namespace CouponManager.Api.Services;

/// <summary>
/// Read-only helper for computing expiration state. Does not modify the database.
/// Rule: a coupon is expired only when ExpirationDate is in the past AND ExpirationType is ExactDate.
/// UntilNotice and RelativeDuration coupons are never auto-expired by date.
/// </summary>
public static class CouponExpirationHelper
{
    public static bool IsExpired(Coupon coupon)
    {
        if (coupon.ExpirationDate is null) return false;
        if (coupon.ExpirationType is ExpirationType.UntilNotice or ExpirationType.RelativeDuration) return false;
        return coupon.ExpirationDate.Value < DateTime.UtcNow;
    }

    public static bool IsEffectivelyActive(Coupon coupon) =>
        coupon.Status == CouponStatus.Active && !IsExpired(coupon);
}
