import { useState } from 'react'

export default function SearchPage() {
  const [query, setQuery] = useState('')

  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">חיפוש</h1>
      <input
        type="search"
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        placeholder="חפש קופון לפי שם, קוד, מוכר..."
        className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 mb-4"
      />
      <div className="bg-white rounded-lg border border-gray-200 p-4">
        <p className="text-gray-400 text-sm text-center">
          {query ? `אין תוצאות עבור "${query}"` : 'הקלד כדי לחפש'}
        </p>
      </div>
    </div>
  )
}
