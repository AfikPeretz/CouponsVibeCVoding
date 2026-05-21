import { apiFetch } from './client'
import type {
  AnalyzeCouponRequest,
  CouponAnalysisResult,
  CreateCouponRequest,
  CouponDto,
  UpdateRemainingAmountRequest,
} from '../types/coupon'

export function analyzeCoupon(request: AnalyzeCouponRequest): Promise<CouponAnalysisResult> {
  return apiFetch<CouponAnalysisResult>('/api/coupons/analyze', {
    method: 'POST',
    body: JSON.stringify(request),
  })
}

export function createCoupon(request: CreateCouponRequest): Promise<CouponDto> {
  return apiFetch<CouponDto>('/api/coupons', {
    method: 'POST',
    body: JSON.stringify(request),
  })
}

export function getCoupons(): Promise<CouponDto[]> {
  return apiFetch<CouponDto[]>('/api/coupons')
}

export function getCouponById(id: number): Promise<CouponDto> {
  return apiFetch<CouponDto>(`/api/coupons/${id}`)
}

export function searchCoupons(query: string): Promise<CouponDto[]> {
  return apiFetch<CouponDto[]>(`/api/coupons/search?query=${encodeURIComponent(query)}`)
}

export function updateRemainingAmount(id: number, request: UpdateRemainingAmountRequest): Promise<CouponDto> {
  return apiFetch<CouponDto>(`/api/coupons/${id}/remaining-amount`, {
    method: 'PATCH',
    body: JSON.stringify(request),
  })
}

export function markCouponUsed(id: number): Promise<CouponDto> {
  return apiFetch<CouponDto>(`/api/coupons/${id}/mark-used`, { method: 'PATCH' })
}

export function archiveCoupon(id: number): Promise<CouponDto> {
  return apiFetch<CouponDto>(`/api/coupons/${id}/archive`, { method: 'PATCH' })
}

export function deleteCoupon(id: number): Promise<void> {
  return apiFetch<void>(`/api/coupons/${id}`, { method: 'DELETE' })
}
