import { apiFetch } from './client'
import type { AnalyzeCouponRequest, CouponAnalysisResult } from '../types/coupon'

export function analyzeCoupon(request: AnalyzeCouponRequest): Promise<CouponAnalysisResult> {
  return apiFetch<CouponAnalysisResult>('/api/coupons/analyze', {
    method: 'POST',
    body: JSON.stringify(request),
  })
}
