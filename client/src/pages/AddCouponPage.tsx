import { useState } from 'react'

export default function AddCouponPage() {
  const [text, setText] = useState('')

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    // TODO: call POST /api/parse with the raw text
    console.log('Submitting:', text)
  }

  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">הוסף קופון</h1>
      <form onSubmit={handleSubmit} className="bg-white rounded-lg border border-gray-200 p-4 space-y-4">
        <div>
          <label htmlFor="raw-text" className="block text-sm font-medium text-gray-700 mb-1">
            הדבק כאן את הודעת ה-SMS / הטקסט הגולמי
          </label>
          <textarea
            id="raw-text"
            value={text}
            onChange={(e) => setText(e.target.value)}
            rows={6}
            placeholder="לדוגמה: קיבלת קופון של 50 ש״ח לרשת מגה..."
            className="w-full border border-gray-300 rounded-md p-3 text-sm resize-none focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>
        <button
          type="submit"
          disabled={!text.trim()}
          className="w-full bg-blue-600 text-white py-2 rounded-md text-sm font-medium hover:bg-blue-700 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
        >
          נתח קופון
        </button>
      </form>
    </div>
  )
}
