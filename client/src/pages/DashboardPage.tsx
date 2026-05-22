import { useQuery } from '@tanstack/react-query'
import { checkHealth } from '../api/healthApi'
import { getCoupons } from '../api/couponApi'
import type { CouponDto } from '../types/coupon'

const ACTIVE_STATUS = 1
const EXPIRING_SOON_DAYS = 14

type Summary = {
  activeCount: number
  totalActiveRemaining: number
  expiringSoonCount: number
  totalCount: number
  lastAdded: CouponDto | null
}

function summarize(coupons: CouponDto[]): Summary {
  const startOfToday = new Date()
  startOfToday.setHours(0, 0, 0, 0)
  const soonCutoff = new Date(startOfToday)
  soonCutoff.setDate(soonCutoff.getDate() + EXPIRING_SOON_DAYS)

  let activeCount = 0
  let totalActiveRemaining = 0
  let expiringSoonCount = 0
  let lastAdded: CouponDto | null = null

  for (const c of coupons) {
    if (c.status === ACTIVE_STATUS) {
      activeCount += 1
      totalActiveRemaining += c.remainingAmount ?? c.originalAmount ?? 0

      if (c.expirationDate) {
        const exp = new Date(c.expirationDate)
        if (exp >= startOfToday && exp <= soonCutoff) {
          expiringSoonCount += 1
        }
      }
    }

    if (!lastAdded || new Date(c.createdAt) > new Date(lastAdded.createdAt)) {
      lastAdded = c
    }
  }

  return {
    activeCount,
    totalActiveRemaining,
    expiringSoonCount,
    totalCount: coupons.length,
    lastAdded,
  }
}

function SummaryCard({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <div className="bg-white rounded-lg border border-gray-200 p-4">
      <p className="text-xs text-gray-500">{label}</p>
      <p className="text-2xl font-bold text-gray-900 mt-1 truncate">{value}</p>
    </div>
  )
}

export default function DashboardPage() {
  const health = useQuery({ queryKey: ['health'], queryFn: checkHealth })
  const coupons = useQuery({ queryKey: ['coupons'], queryFn: getCoupons })

  const summary = coupons.data ? summarize(coupons.data) : null

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">דשבורד</h1>

      {coupons.isLoading && (
        <p className="text-sm text-gray-400 text-center py-6">טוען נתונים...</p>
      )}

      {coupons.isError && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-sm text-red-700">
          שגיאה בטעינת השוברים. ודא שהשרת פעיל ונסה שנית.
        </div>
      )}

      {summary && summary.totalCount === 0 && (
        <div className="bg-white rounded-lg border border-gray-200 p-8 text-center">
          <p className="text-gray-400 text-sm">אין עדיין שוברים שמורים</p>
        </div>
      )}

      {summary && summary.totalCount > 0 && (
        <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
          <SummaryCard label="שוברים פעילים" value={summary.activeCount} />
          <SummaryCard label="סה״כ יתרה פעילה" value={`₪${summary.totalActiveRemaining}`} />
          <SummaryCard label="שוברים שפגים בקרוב" value={summary.expiringSoonCount} />
          <SummaryCard label="סה״כ שוברים" value={summary.totalCount} />
          {summary.lastAdded && (
            <div className="col-span-2 sm:col-span-3 bg-white rounded-lg border border-gray-200 p-4">
              <p className="text-xs text-gray-500">שובר אחרון שנוסף</p>
              <p className="text-base font-semibold text-gray-900 mt-1 truncate">
                {summary.lastAdded.title ?? 'שובר'}
              </p>
              {summary.lastAdded.merchantName && (
                <p className="text-xs text-gray-500 truncate">{summary.lastAdded.merchantName}</p>
              )}
            </div>
          )}
        </div>
      )}

      <div className="bg-white rounded-lg border border-gray-200 p-4">
        <h2 className="text-sm font-medium text-gray-500 mb-1">סטטוס שרת</h2>
        {health.isLoading && <p className="text-gray-400 text-sm">בודק חיבור...</p>}
        {health.isError && <p className="text-red-500 text-sm">שגיאה בחיבור לשרת</p>}
        {health.data && (
          <p className="text-green-600 text-sm font-medium">
            ✓ שרת פעיל — {health.data.status}
          </p>
        )}
      </div>
    </div>
  )
}
