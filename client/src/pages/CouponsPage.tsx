import { useQuery } from '@tanstack/react-query'
import { getCoupons } from '../api/couponApi'
import {
  CATEGORY_LABELS,
  EXPIRY_TYPE_LABELS,
  STATUS_LABELS,
  type CouponDto,
} from '../types/coupon'

function formatDate(iso: string | null): string {
  if (!iso) return ''
  return iso.split('T')[0]
}

function CouponCard({ coupon }: { coupon: CouponDto }) {
  const expiryDisplay = coupon.expirationDate
    ? formatDate(coupon.expirationDate)
    : (coupon.expirationText ?? '—')

  const amountDisplay =
    coupon.remainingAmount != null
      ? `${coupon.remainingAmount} ${coupon.currency ?? ''}`
      : coupon.originalAmount != null
      ? `${coupon.originalAmount} ${coupon.currency ?? ''}`
      : null

  const statusColor: Record<number, string> = {
    1: 'bg-green-100 text-green-800',
    2: 'bg-red-100 text-red-800',
    3: 'bg-gray-100 text-gray-600',
  }
  const statusClass = statusColor[coupon.status] ?? 'bg-gray-100 text-gray-600'

  return (
    <div className="bg-white rounded-lg border border-gray-200 p-4 space-y-3">
      {/* Header row */}
      <div className="flex justify-between items-start gap-2">
        <div className="min-w-0">
          <p className="font-semibold text-gray-900 truncate">{coupon.title ?? 'קופון'}</p>
          <p className="text-sm text-gray-500 truncate">
            {coupon.merchantName ?? '—'}
            {coupon.provider ? ` · ${coupon.provider}` : ''}
          </p>
        </div>
        <span className={`shrink-0 text-xs font-medium px-2 py-0.5 rounded-full ${statusClass}`}>
          {STATUS_LABELS[coupon.status] ?? 'לא ידוע'}
        </span>
      </div>

      {/* Amount */}
      {amountDisplay && (
        <p className="text-2xl font-bold text-blue-700">₪{amountDisplay}</p>
      )}

      {/* Meta row */}
      <div className="flex flex-wrap gap-x-4 gap-y-1 text-xs text-gray-500">
        <span>קטגוריה: {CATEGORY_LABELS[coupon.category] ?? '—'}</span>
        <span>תפוגה: {expiryDisplay} ({EXPIRY_TYPE_LABELS[coupon.expirationType] ?? '—'})</span>
        <span>בטחון: {Math.round(coupon.confidence * 100)}%</span>
      </div>

      {/* Coupon code */}
      {coupon.couponCode && (
        <div className="flex items-center gap-2">
          <span className="text-xs text-gray-500">קוד:</span>
          <code className="text-xs bg-gray-100 rounded px-2 py-0.5 font-mono tracking-wide dir-ltr">
            {coupon.couponCode}
          </code>
        </div>
      )}

      {/* Voucher URL */}
      {coupon.voucherUrl && (
        <a
          href={coupon.voucherUrl}
          target="_blank"
          rel="noopener noreferrer"
          className="block text-xs text-blue-600 hover:underline truncate"
          dir="ltr"
        >
          {coupon.voucherUrl}
        </a>
      )}
    </div>
  )
}

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
