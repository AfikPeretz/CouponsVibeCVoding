// ── Shared label maps ────────────────────────────────────────────────────────

export const CATEGORY_LABELS: Record<number, string> = {
  0: 'לא ידוע', 1: 'מזון', 2: 'אופנה', 3: 'נסיעות',
  4: 'בריאות', 5: 'בידור', 6: 'סופרמרקט', 7: 'רב-מותגי',
  8: 'תחנת דלק', 9: 'כרטיס מתנה', 99: 'אחר',
}

export const EXPIRY_TYPE_LABELS: Record<number, string> = {
  0: 'לא ידוע', 1: 'תאריך מדויק', 2: 'עד הודעה חדשה', 3: 'משך יחסי',
}

export const STATUS_LABELS: Record<number, string> = {
  0: 'לא ידוע', 1: 'פעיל', 2: 'פג תוקף', 3: 'מומש',
}

// ── Categories ───────────────────────────────────────────────────────────────

export interface CategorySummary {
  category: number
  displayName: string
  totalCoupons: number
  activeCoupons: number
  totalActiveRemainingAmount: number | null
}

// ── Analyze ──────────────────────────────────────────────────────────────────

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

// ── Save / CRUD ───────────────────────────────────────────────────────────────

// Request sent to POST /api/coupons
export interface CreateCouponRequest {
  rawText: string
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
  expirationDate: string | null
  expirationText: string | null
  expirationType: number
  onlineRedeemable: boolean
  status: number
  confidence: number
  conditionsText: string | null
}

// Response from GET /api/coupons and GET /api/coupons/{id}
export interface CouponDto {
  id: number
  rawText: string
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
  expirationDate: string | null
  expirationText: string | null
  expirationType: number
  onlineRedeemable: boolean
  status: number
  confidence: number
  conditionsText: string | null
  receivedAt: string | null
  createdAt: string
  updatedAt: string
}
