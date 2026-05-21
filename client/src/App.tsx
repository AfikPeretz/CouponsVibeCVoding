import { Routes, Route, Navigate } from 'react-router-dom'
import Layout from './components/Layout'
import DashboardPage from './pages/DashboardPage'
import CouponsPage from './pages/CouponsPage'
import AddCouponPage from './pages/AddCouponPage'
import SearchPage from './pages/SearchPage'
import CategoriesPage from './pages/CategoriesPage'

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<Navigate to="/dashboard" replace />} />
        <Route path="dashboard" element={<DashboardPage />} />
        <Route path="coupons" element={<CouponsPage />} />
        <Route path="add" element={<AddCouponPage />} />
        <Route path="search" element={<SearchPage />} />
        <Route path="categories" element={<CategoriesPage />} />
      </Route>
    </Routes>
  )
}
