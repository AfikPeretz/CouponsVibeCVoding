const PLACEHOLDER_CATEGORIES = [
  { id: 'food', label: 'מזון ומסעדות', count: 0 },
  { id: 'fashion', label: 'אופנה', count: 0 },
  { id: 'travel', label: 'נסיעות ותיירות', count: 0 },
  { id: 'health', label: 'בריאות וספא', count: 0 },
  { id: 'other', label: 'אחר', count: 0 },
]

export default function CategoriesPage() {
  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">קטגוריות</h1>
      <ul className="space-y-2">
        {PLACEHOLDER_CATEGORIES.map((cat) => (
          <li
            key={cat.id}
            className="bg-white rounded-lg border border-gray-200 px-4 py-3 flex justify-between items-center"
          >
            <span className="text-sm font-medium text-gray-800">{cat.label}</span>
            <span className="text-xs text-gray-400">{cat.count} קופונים</span>
          </li>
        ))}
      </ul>
    </div>
  )
}
