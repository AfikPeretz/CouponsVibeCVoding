import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { getCategories, getCouponsByCategory } from '../api/categoryApi'
import CouponCard from '../components/CouponCard'
import type { CategorySummary } from '../types/coupon'

function CategoryCard({
  summary,
  onClick,
}: {
  summary: CategorySummary
  onClick: () => void
}) {
  return (
    <button
      onClick={onClick}
      className="w-full text-right bg-white rounded-lg border border-gray-200 px-4 py-3
                 hover:border-blue-400 hover:shadow-sm transition-all flex justify-between items-center gap-2"
    >
      <div className="min-w-0">
        <p className="font-medium text-gray-800 text-sm">{summary.displayName}</p>
        <p className="text-xs text-gray-400 mt-0.5">
          {summary.activeCoupons} פעיל / {summary.totalCoupons} סה"כ
          {summary.totalActiveRemainingAmount != null && (
            <> · יתרה: ₪{summary.totalActiveRemainingAmount}</>
          )}
        </p>
      </div>
      <span className="shrink-0 text-xs bg-blue-50 text-blue-700 font-medium px-2 py-0.5 rounded-full">
        {summary.totalCoupons}
      </span>
    </button>
  )
}

export default function CategoriesPage() {
  const [selectedCategory, setSelectedCategory] = useState<number | null>(null)

  const { data: categories, isLoading: catLoading, isError: catError } = useQuery({
    queryKey: ['categories'],
    queryFn: getCategories,
  })

  const selectedSummary = categories?.find((c) => c.category === selectedCategory)

  const {
    data: coupons,
    isLoading: couponsLoading,
    isError: couponsError,
  } = useQuery({
    queryKey: ['categories', selectedCategory, 'coupons'],
    queryFn: () => getCouponsByCategory(selectedCategory!),
    enabled: selectedCategory !== null,
  })

  // ── Category list ──────────────────────────────────────────────────────────
  if (selectedCategory === null) {
    return (
      <div className="space-y-4">
        <h1 className="text-2xl font-bold">קטגוריות</h1>

        {catLoading && (
          <p className="text-sm text-gray-400 text-center py-8">טוען קטגוריות...</p>
        )}

        {catError && (
          <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-sm text-red-700">
            שגיאה בטעינת הקטגוריות. ודא שהשרת פעיל ונסה שנית.
          </div>
        )}

        {categories && categories.length === 0 && (
          <div className="bg-white rounded-lg border border-gray-200 p-8 text-center">
            <p className="text-gray-400 text-sm">אין עדיין קופונים שמורים</p>
          </div>
        )}

        {categories && categories.length > 0 && (
          <ul className="space-y-2">
            {categories.map((summary) => (
              <li key={summary.category}>
                <CategoryCard
                  summary={summary}
                  onClick={() => setSelectedCategory(summary.category)}
                />
              </li>
            ))}
          </ul>
        )}
      </div>
    )
  }

  // ── Coupon list for selected category ─────────────────────────────────────
  return (
    <div className="space-y-4">
      <div className="flex items-center gap-3">
        <button
          onClick={() => setSelectedCategory(null)}
          className="text-sm text-blue-600 hover:underline"
        >
          ← קטגוריות
        </button>
        <h1 className="text-2xl font-bold">{selectedSummary?.displayName ?? 'קטגוריה'}</h1>
      </div>

      {couponsLoading && (
        <p className="text-sm text-gray-400 text-center py-8">טוען קופונים...</p>
      )}

      {couponsError && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-sm text-red-700">
          שגיאה בטעינת הקופונים.
        </div>
      )}

      {coupons && coupons.length === 0 && (
        <div className="bg-white rounded-lg border border-gray-200 p-8 text-center">
          <p className="text-gray-400 text-sm">אין קופונים בקטגוריה זו</p>
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
