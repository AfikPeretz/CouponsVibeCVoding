import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { analyzeCoupon, createCoupon } from '../api/couponApi'
import { ApiError } from '../api/client'
import {
  CATEGORY_LABELS,
  EXPIRY_TYPE_LABELS,
  type CouponAnalysisResult,
  type CreateCouponRequest,
} from '../types/coupon'

// Fake example — development/testing only, not a real coupon.
const FAKE_EXAMPLE_TEXT =
  'לצפיה בשובר שופרסל בסך ₪30.00: https://example.com/fake-voucher ' +
  'לא ניתן לקבל עודף מהשובר. לא ניתן לממש באונליין. ' +
  'השובר בתוקף עד הודעה חדשה.'

function toDateInput(iso: string | null): string {
  if (!iso) return ''
  return iso.split('T')[0]
}

function toCreateRequest(rawText: string, r: CouponAnalysisResult): CreateCouponRequest {
  return {
    rawText,
    title:                  r.title,
    provider:               r.provider,
    merchantName:           r.merchantName,
    normalizedMerchantName: r.normalizedMerchantName,
    category:               r.category,
    originalAmount:         r.originalAmount,
    remainingAmount:        r.remainingAmount,
    currency:               r.currency ?? 'ILS',
    couponCode:             r.couponCode,
    numerator:              r.numerator,
    voucherUrl:             r.voucherUrl,
    expirationDate:         r.expirationDate,
    expirationText:         r.expirationText,
    expirationType:         r.expirationType,
    onlineRedeemable:       r.onlineRedeemable,
    status:                 r.status,
    confidence:             r.confidence,
    conditionsText:         r.conditionsText,
  }
}

function FieldRow({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div className="flex flex-col gap-1">
      <span className="text-xs font-medium text-gray-500">{label}</span>
      {children}
    </div>
  )
}

const inputClass =
  'w-full border border-gray-300 rounded-md px-3 py-2 text-sm ' +
  'focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white'

const readonlyClass =
  'w-full border border-gray-200 rounded-md px-3 py-2 text-sm bg-gray-50 text-gray-600'

export default function AddCouponPage() {
  const navigate     = useNavigate()
  const queryClient  = useQueryClient()
  const [rawText, setRawText] = useState('')
  const [result,  setResult]  = useState<CouponAnalysisResult | null>(null)

  const analyzeMutation = useMutation({
    mutationFn: analyzeCoupon,
    onSuccess: (data) => setResult(data),
  })

  const saveMutation = useMutation({
    mutationFn: createCoupon,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['coupons'] })
      navigate('/coupons')
    },
  })

  function handleAnalyze(e: React.FormEvent) {
    e.preventDefault()
    setResult(null)
    saveMutation.reset()
    analyzeMutation.mutate({ rawText })
  }

  function handleSave() {
    if (!result) return
    saveMutation.mutate(toCreateRequest(rawText, result))
  }

  function updateField<K extends keyof CouponAnalysisResult>(
    key: K,
    value: CouponAnalysisResult[K],
  ) {
    setResult((prev) => (prev ? { ...prev, [key]: value } : prev))
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold mb-1">הוסף שובר חדש</h1>
        <p className="text-sm text-gray-500">
          הדבק הודעת SMS או טקסט גולמי שמכיל שובר. המערכת תנתח את הטקסט ותחלץ את הפרטים לעיון ועריכה.
        </p>
      </div>

      {/* ── Input section ─────────────────────────────────────────────── */}
      <form onSubmit={handleAnalyze} className="bg-white rounded-lg border border-gray-200 p-4 space-y-3">
        <div className="flex justify-between items-center">
          <label htmlFor="raw-text" className="text-sm font-medium text-gray-700">
            טקסט גולמי
          </label>
          <button
            type="button"
            onClick={() => setRawText(FAKE_EXAMPLE_TEXT)}
            className="text-xs text-blue-600 hover:underline"
          >
            טען דוגמה
          </button>
        </div>

        <textarea
          id="raw-text"
          value={rawText}
          onChange={(e) => setRawText(e.target.value)}
          rows={5}
          placeholder="לדוגמה: לצפיה בשובר שופרסל בסך ₪30.00..."
          className={inputClass + ' resize-none'}
        />

        <button
          type="submit"
          disabled={!rawText.trim() || analyzeMutation.isPending}
          className="w-full bg-blue-600 text-white py-2 rounded-md text-sm font-medium
                     hover:bg-blue-700 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
        >
          {analyzeMutation.isPending ? 'מנתח...' : 'נתח שובר'}
        </button>
      </form>

      {/* ── Analyze error ─────────────────────────────────────────────── */}
      {analyzeMutation.isError && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-sm text-red-700">
          שגיאה בניתוח השובר. ודא שהשרת פעיל ונסה שנית.
          {analyzeMutation.error instanceof Error && (
            <p className="mt-1 text-xs text-red-500">{analyzeMutation.error.message}</p>
          )}
        </div>
      )}

      {/* ── Parsed result preview ─────────────────────────────────────── */}
      {result && (
        <div className="bg-white rounded-lg border border-gray-200 p-4 space-y-5">
          <div className="flex justify-between items-center">
            <h2 className="text-base font-semibold text-gray-800">תוצאת ניתוח</h2>
            <span className="text-xs text-gray-400">
              ציון בטחון: {Math.round(result.confidence * 100)}%
            </span>
          </div>

          {result.warnings.length > 0 && (
            <div className="bg-yellow-50 border border-yellow-200 rounded-md p-3 space-y-1">
              <p className="text-xs font-medium text-yellow-800">אזהרות:</p>
              {result.warnings.map((w, i) => (
                <p key={i} className="text-xs text-yellow-700">• {w}</p>
              ))}
            </div>
          )}

          {/* ── זיהוי ────────────────────────────────────────────────── */}
          <fieldset className="space-y-3">
            <legend className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-2">זיהוי</legend>
            <FieldRow label="כותרת">
              <input type="text" value={result.title ?? ''}
                onChange={(e) => updateField('title', e.target.value || null)}
                className={inputClass} />
            </FieldRow>
            <FieldRow label="ספק">
              <input type="text" value={result.provider ?? ''}
                onChange={(e) => updateField('provider', e.target.value || null)}
                className={inputClass} />
            </FieldRow>
            <FieldRow label="שם בית עסק">
              <input type="text" value={result.merchantName ?? ''}
                onChange={(e) => updateField('merchantName', e.target.value || null)}
                className={inputClass} />
            </FieldRow>
            <FieldRow label="שם מנורמל">
              <input type="text" value={result.normalizedMerchantName ?? ''}
                onChange={(e) => updateField('normalizedMerchantName', e.target.value || null)}
                className={inputClass} />
            </FieldRow>
            <FieldRow label="קטגוריה">
              <select value={result.category}
                onChange={(e) => updateField('category', parseInt(e.target.value))}
                className={inputClass}>
                {Object.entries(CATEGORY_LABELS).map(([v, l]) => (
                  <option key={v} value={v}>{l}</option>
                ))}
              </select>
            </FieldRow>
          </fieldset>

          {/* ── ערך ──────────────────────────────────────────────────── */}
          <fieldset className="space-y-3">
            <legend className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-2">ערך</legend>
            <div className="grid grid-cols-2 gap-3">
              <FieldRow label="סכום מקורי">
                <input type="number" step="0.01" value={result.originalAmount ?? ''}
                  onChange={(e) => updateField('originalAmount', e.target.value ? parseFloat(e.target.value) : null)}
                  className={inputClass} />
              </FieldRow>
              <FieldRow label="יתרה">
                <input type="number" step="0.01" value={result.remainingAmount ?? ''}
                  onChange={(e) => updateField('remainingAmount', e.target.value ? parseFloat(e.target.value) : null)}
                  className={inputClass} />
              </FieldRow>
            </div>
            <FieldRow label="מטבע">
              <input type="text" value={result.currency ?? ''}
                onChange={(e) => updateField('currency', e.target.value || null)}
                className={inputClass} />
            </FieldRow>
          </fieldset>

          {/* ── מימוש ────────────────────────────────────────────────── */}
          <fieldset className="space-y-3">
            <legend className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-2">מימוש</legend>
            <FieldRow label="קוד שובר">
              <input type="text" value={result.couponCode ?? ''}
                onChange={(e) => updateField('couponCode', e.target.value || null)}
                className={inputClass} />
            </FieldRow>
            <FieldRow label="נומרטור">
              <input type="text" value={result.numerator ?? ''}
                onChange={(e) => updateField('numerator', e.target.value || null)}
                className={inputClass} />
            </FieldRow>
            <FieldRow label="קישור מימוש">
              <input type="url" value={result.voucherUrl ?? ''}
                onChange={(e) => updateField('voucherUrl', e.target.value || null)}
                className={inputClass} dir="ltr" />
            </FieldRow>
            <FieldRow label="ניתן לממש אונליין">
              <label className="flex items-center gap-2 cursor-pointer">
                <input type="checkbox" checked={result.onlineRedeemable}
                  onChange={(e) => updateField('onlineRedeemable', e.target.checked)}
                  className="w-4 h-4 rounded border-gray-300 text-blue-600" />
                <span className="text-sm text-gray-700">{result.onlineRedeemable ? 'כן' : 'לא'}</span>
              </label>
            </FieldRow>
          </fieldset>

          {/* ── תפוגה ────────────────────────────────────────────────── */}
          <fieldset className="space-y-3">
            <legend className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-2">תפוגה</legend>
            <FieldRow label="תאריך תפוגה">
              <input type="date" value={toDateInput(result.expirationDate)}
                onChange={(e) =>
                  updateField('expirationDate', e.target.value ? e.target.value + 'T00:00:00Z' : null)}
                className={inputClass} dir="ltr" />
            </FieldRow>
            <FieldRow label="טקסט תפוגה">
              <input type="text" value={result.expirationText ?? ''}
                onChange={(e) => updateField('expirationText', e.target.value || null)}
                className={inputClass} />
            </FieldRow>
            <FieldRow label="סוג תפוגה">
              <select value={result.expirationType}
                onChange={(e) => updateField('expirationType', parseInt(e.target.value))}
                className={inputClass}>
                {Object.entries(EXPIRY_TYPE_LABELS).map(([v, l]) => (
                  <option key={v} value={v}>{l}</option>
                ))}
              </select>
            </FieldRow>
          </fieldset>

          {/* ── תנאים ────────────────────────────────────────────────── */}
          <fieldset className="space-y-3">
            <legend className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-2">תנאים</legend>
            <FieldRow label="טקסט תנאים">
              <textarea rows={3} value={result.conditionsText ?? ''}
                onChange={(e) => updateField('conditionsText', e.target.value || null)}
                className={inputClass + ' resize-none'} />
            </FieldRow>
          </fieldset>

          {/* ── פרטי ניתוח (readonly) ────────────────────────────────── */}
          <fieldset className="space-y-3">
            <legend className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-2">פרטי ניתוח</legend>
            <FieldRow label="ציון בטחון">
              <input type="text" readOnly value={`${Math.round(result.confidence * 100)}%`}
                className={readonlyClass} />
            </FieldRow>
            <FieldRow label="אזהרות">
              <div className={readonlyClass + ' min-h-[2.5rem]'}>
                {result.warnings.length === 0
                  ? <span className="text-gray-400">אין אזהרות</span>
                  : <ul className="space-y-1">
                      {result.warnings.map((w, i) => (
                        <li key={i} className="text-yellow-700">• {w}</li>
                      ))}
                    </ul>
                }
              </div>
            </FieldRow>
          </fieldset>

          {/* ── Save error ─────────────────────────────────────────────── */}
          {saveMutation.isError && (
            <div className="bg-red-50 border border-red-200 rounded-md p-3 text-sm text-red-700">
              {saveMutation.error instanceof ApiError && saveMutation.error.status === 409 ? (
                'שובר זה כבר קיים במערכת'
              ) : (
                <>
                  שגיאה בשמירת השובר. נסה שנית.
                  {saveMutation.error instanceof Error && (
                    <p className="mt-1 text-xs text-red-500">{saveMutation.error.message}</p>
                  )}
                </>
              )}
            </div>
          )}

          {/* ── Save button ─────────────────────────────────────────────── */}
          <button
            type="button"
            onClick={handleSave}
            disabled={saveMutation.isPending}
            className="w-full bg-green-600 text-white py-2 rounded-md text-sm font-medium
                       hover:bg-green-700 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
          >
            {saveMutation.isPending ? 'שומר...' : 'שמור שובר'}
          </button>
        </div>
      )}
    </div>
  )
}
