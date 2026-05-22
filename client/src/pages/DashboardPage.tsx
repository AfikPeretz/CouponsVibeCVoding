import { useQuery } from '@tanstack/react-query'
import { checkHealth } from '../api/healthApi'

export default function DashboardPage() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ['health'],
    queryFn: checkHealth,
  })

  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">דשבורד</h1>

      <div className="bg-white rounded-lg border border-gray-200 p-4 mb-4">
        <h2 className="text-sm font-medium text-gray-500 mb-1">סטטוס שרת</h2>
        {isLoading && <p className="text-gray-400 text-sm">בודק חיבור...</p>}
        {isError && <p className="text-red-500 text-sm">שגיאה בחיבור לשרת</p>}
        {data && (
          <p className="text-green-600 text-sm font-medium">
            ✓ שרת פעיל — {data.status}
          </p>
        )}
      </div>
    </div>
  )
}
