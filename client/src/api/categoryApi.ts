import { apiFetch } from './client'
import type { CategorySummary, CouponDto } from '../types/coupon'

export function getCategories(): Promise<CategorySummary[]> {
  return apiFetch<CategorySummary[]>('/api/categories')
}

export function getCouponsByCategory(category: number): Promise<CouponDto[]> {
  return apiFetch<CouponDto[]>(`/api/categories/${category}/coupons`)
}
