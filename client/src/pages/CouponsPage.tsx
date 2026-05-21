import { useQuery } from '@tanstack/react-query'
import { getCoupons } from '../api/couponApi'
import CouponCard from '../components/CouponCard'

export default function CouponsPage() {
  const { data: coupons, isLoading, isError, error } = useQuery({
    queryKey: ['coupons'],
    queryFn: getCoupons,
  })

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">הקופונים שלי</h1>

      {isLoading && (
        <p className="text-sm text-gray-400 text-center py-8">טוען קופונים...</p>
      )}

      {isError && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-sm text-red-700">
          שגיאה בטעינת הקופונים. ודא שהשרת פעיל ונסה שנית.
          {error instanceof Error && (
            <p className="mt-1 text-xs text-red-500">{error.message}</p>
          )}
        </div>
      )}

      {coupons && coupons.length === 0 && (
        <div className="bg-white rounded-lg border border-gray-200 p-8 text-center">
          <p className="text-gray-400 text-sm">אין עדיין קופונים שמורים</p>
        </div>
      )}

      {coupons && coupons.length > 0 && (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {coupons.map((coupon) => (
            <CouponCard key={coupon.id} coupon={coupon} />
          ))}
        </div>
      )}
    </div>
  )
}
