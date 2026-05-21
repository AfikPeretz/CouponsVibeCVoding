import { NavLink } from 'react-router-dom'

const links = [
  { to: '/dashboard', label: 'דשבורד' },
  { to: '/coupons', label: 'קופונים' },
  { to: '/add', label: '+ הוסף' },
  { to: '/search', label: 'חיפוש' },
  { to: '/categories', label: 'קטגוריות' },
]

export default function NavBar() {
  return (
    <nav className="bg-white border-b border-gray-200 sticky top-0 z-10">
      <div className="max-w-2xl mx-auto px-4 flex items-center gap-1 overflow-x-auto">
        {links.map(({ to, label }) => (
          <NavLink
            key={to}
            to={to}
            className={({ isActive }) =>
              [
                'shrink-0 px-3 py-4 text-sm font-medium transition-colors',
                isActive
                  ? 'text-blue-600 border-b-2 border-blue-600'
                  : 'text-gray-500 hover:text-gray-800',
              ].join(' ')
            }
          >
            {label}
          </NavLink>
        ))}
      </div>
    </nav>
  )
}
