import { apiFetch } from './client'
import type {
  AnalyzeCouponRequest,
  CouponAnalysisResult,
  CreateCouponRequest,
  CouponDto,
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
