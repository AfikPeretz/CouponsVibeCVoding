// Request sent to POST /api/coupons/analyze
export interface AnalyzeCouponRequest {
  rawText: string
}

// Response from POST /api/coupons/analyze
// Enum fields (category, expirationType, status) are numeric values from the backend.
export interface CouponAnalysisResult {
  title: string | null
  provider: string | null
  merchantName: string | null
  normalizedMerchantName: string | null
  category: number
  originalAmount: number | null
  remainingAmount: number | null
  currency: string | null
  couponCode: string | null
  numerator: string | null
  voucherUrl: string | null
  expirationDate: string | null   // ISO 8601 date string or null
  expirationText: string | null
  expirationType: number
  onlineRedeemable: boolean
  status: number
  confidence: number              // 0.0–1.0
  conditionsText: string | null
  warnings: string[]
}
