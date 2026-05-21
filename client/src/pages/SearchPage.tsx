import { useState, useEffect } from 'react'
import { useQuery } from '@tanstack/react-query'
import { searchCoupons } from '../api/couponApi'
import CouponCard from '../components/CouponCard'

export default function SearchPage() {
  const [inputValue, setInputValue] = useState('')
  const [debouncedQuery, setDebouncedQuery] = useState('')

  // Debounce: wait 300 ms after the user stops typing before firing the request.
  useEffect(() => {
    const timer = setTimeout(() => setDebouncedQuery(inputValue.trim()), 300)
    return () => clearTimeout(timer)
  }, [inputValue])

  const { data: results, isLoading, isError, error } = useQuery({
    queryKey: ['coupons', 'search', debouncedQuery],
    queryFn: () => searchCoupons(debouncedQuery),
    enabled: debouncedQuery.length > 0,
  })

  const hasQuery = debouncedQuery.length > 0

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">חיפוש</h1>

      <input
        type="search"
        value={inputValue}
        onChange={(e) => setInputValue(e.target.value)}
        placeholder="חפש לפי שם בית עסק, ספק, קוד..."
        className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm
                   focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
        autoFocus
      />

      {/* Idle — no query yet */}
      {!hasQuery && (
        <div className="bg-white rounded-lg border border-gray-200 p-8 text-center">
          <p className="text-gray-400 text-sm">הקלד כדי לחפש</p>
        </div>
      )}

      {/* Loading */}
      {hasQuery && isLoading && (
        <p className="text-sm text-gray-400 text-center py-6">מחפש...</p>
      )}

      {/* Error */}
      {hasQuery && isError && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-sm text-red-700">
          שגיאה בחיפוש. ודא שהשרת פעיל ונסה שנית.
          {error instanceof Error && (
            <p className="mt-1 text-xs text-red-500">{error.message}</p>
          )}
        </div>
      )}

      {/* Empty results */}
      {hasQuery && !isLoading && results && results.length === 0 && (
        <div className="bg-white rounded-lg border border-gray-200 p-8 text-center">
          <p className="text-gray-500 text-sm">לא נמצאו קופונים מתאימים</p>
          <p className="text-gray-400 text-xs mt-1">נסה מונח אחר</p>
        </div>
      )}

      {/* Results */}
      {results && results.length > 0 && (
        <>
          <p className="text-xs text-gray-400">{results.length} תוצאות</p>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {results.map((coupon) => (
              <CouponCard key={coupon.id} coupon={coupon} />
            ))}
          </div>
        </>
      )}
    </div>
  )
}
