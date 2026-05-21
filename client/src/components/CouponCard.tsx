import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import {
  markCouponUsed,
  archiveCoupon,
  deleteCoupon,
  updateRemainingAmount,
} from '../api/couponApi'
import { CATEGORY_LABELS, EXPIRY_TYPE_LABELS, STATUS_LABELS, type CouponDto } from '../types/coupon'

function formatDate(iso: string | null): string {
  if (!iso) return ''
  return iso.split('T')[0]
}

const statusColor: Record<number, string> = {
  1: 'bg-green-100 text-green-800',
  2: 'bg-red-100 text-red-800',
  3: 'bg-gray-100 text-gray-600',
  4: 'bg-purple-100 text-purple-700',
}

const btnBase =
  'text-xs px-2.5 py-1 rounded-md font-medium transition-colors disabled:opacity-40 disabled:cursor-not-allowed'

export default function CouponCard({ coupon }: { coupon: CouponDto }) {
  const queryClient = useQueryClient()
  const [editingAmount, setEditingAmount] = useState(false)
  const [amountInput, setAmountInput] = useState(
    String(coupon.remainingAmount ?? coupon.originalAmount ?? ''),
  )

  // Invalidate all coupon and category queries so every page refreshes.
  function invalidate() {
    queryClient.invalidateQueries({ queryKey: ['coupons'] })
    queryClient.invalidateQueries({ queryKey: ['categories'] })
  }

  const markUsedMutation = useMutation({
    mutationFn: () => markCouponUsed(coupon.id),
    onSuccess: invalidate,
  })

  const archiveMutation = useMutation({
    mutationFn: () => archiveCoupon(coupon.id),
    onSuccess: invalidate,
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteCoupon(coupon.id),
    onSuccess: invalidate,
  })

  const updateAmountMutation = useMutation({
    mutationFn: (amount: number) => updateRemainingAmount(coupon.id, { remainingAmount: amount }),
    onSuccess: () => {
      setEditingAmount(false)
      invalidate()
    },
  })

  function handleDelete() {
    if (window.confirm(`למחוק את "${coupon.title ?? 'קופון'}"?`)) {
      deleteMutation.mutate()
    }
  }

  function handleConfirmAmount() {
    const val = parseFloat(amountInput)
    if (isNaN(val) || val < 0) return
    updateAmountMutation.mutate(val)
  }

  const anyPending =
    markUsedMutation.isPending ||
    archiveMutation.isPending ||
    deleteMutation.isPending ||
    updateAmountMutation.isPending

  // ── Derived display values ─────────────────────────────────────────────────
  const expiryDisplay = coupon.expirationDate
    ? formatDate(coupon.expirationDate)
    : (coupon.expirationText ?? '—')

  const amountDisplay =
    coupon.remainingAmount != null
      ? `${coupon.remainingAmount} ${coupon.currency ?? ''}`
      : coupon.originalAmount != null
      ? `${coupon.originalAmount} ${coupon.currency ?? ''}`
      : null

  const statusClass = statusColor[coupon.status] ?? 'bg-gray-100 text-gray-600'
  const hasAmount  = coupon.remainingAmount != null || coupon.originalAmount != null
  const isActive   = coupon.status === 1
  const isArchived = coupon.status === 4

  return (
    <div className="bg-white rounded-lg border border-gray-200 p-4 space-y-3">
      {/* ── Header ────────────────────────────────────────────────────────── */}
      <div className="flex justify-between items-start gap-2">
        <div className="min-w-0">
          <p className="font-semibold text-gray-900 truncate">{coupon.title ?? 'קופון'}</p>
          <p className="text-sm text-gray-500 truncate">
            {coupon.merchantName ?? '—'}
            {coupon.provider ? ` · ${coupon.provider}` : ''}
          </p>
        </div>
        <span className={`shrink-0 text-xs font-medium px-2 py-0.5 rounded-full ${statusClass}`}>
          {STATUS_LABELS[coupon.status] ?? 'לא ידוע'}
        </span>
      </div>

      {/* ── Amount ────────────────────────────────────────────────────────── */}
      {amountDisplay && (
        <p className="text-2xl font-bold text-blue-700">₪{amountDisplay}</p>
      )}

      {/* ── Meta ──────────────────────────────────────────────────────────── */}
      <div className="flex flex-wrap gap-x-4 gap-y-1 text-xs text-gray-500">
        <span>קטגוריה: {CATEGORY_LABELS[coupon.category] ?? '—'}</span>
        <span>תפוגה: {expiryDisplay} ({EXPIRY_TYPE_LABELS[coupon.expirationType] ?? '—'})</span>
        <span>בטחון: {Math.round(coupon.confidence * 100)}%</span>
      </div>

      {/* ── Coupon code ───────────────────────────────────────────────────── */}
      {coupon.couponCode && (
        <div className="flex items-center gap-2">
          <span className="text-xs text-gray-500">קוד:</span>
          <code className="text-xs bg-gray-100 rounded px-2 py-0.5 font-mono tracking-wide" dir="ltr">
            {coupon.couponCode}
          </code>
        </div>
      )}

      {/* ── Voucher URL ───────────────────────────────────────────────────── */}
      {coupon.voucherUrl && (
        <a
          href={coupon.voucherUrl}
          target="_blank"
          rel="noopener noreferrer"
          className="block text-xs text-blue-600 hover:underline truncate"
          dir="ltr"
        >
          {coupon.voucherUrl}
        </a>
      )}

      {/* ── Mutation error ────────────────────────────────────────────────── */}
      {(markUsedMutation.isError || archiveMutation.isError ||
        deleteMutation.isError || updateAmountMutation.isError) && (
        <p className="text-xs text-red-600">שגיאה בביצוע הפעולה. נסה שנית.</p>
      )}

      {/* ── Action bar ────────────────────────────────────────────────────── */}
      <div className="border-t border-gray-100 pt-3">
        {editingAmount ? (
          /* Inline remaining-amount editor */
          <div className="flex items-center gap-2">
            <input
              type="number"
              min="0"
              step="0.01"
              value={amountInput}
              onChange={(e) => setAmountInput(e.target.value)}
              className="w-24 border border-gray-300 rounded-md px-2 py-1 text-sm
                         focus:outline-none focus:ring-2 focus:ring-blue-500"
              dir="ltr"
              autoFocus
            />
            <button
              onClick={handleConfirmAmount}
              disabled={updateAmountMutation.isPending}
              className={`${btnBase} bg-blue-600 text-white hover:bg-blue-700`}
            >
              {updateAmountMutation.isPending ? '...' : 'אשר'}
            </button>
            <button
              onClick={() => setEditingAmount(false)}
              disabled={updateAmountMutation.isPending}
              className={`${btnBase} bg-gray-100 text-gray-700 hover:bg-gray-200`}
            >
              ביטול
            </button>
          </div>
        ) : (
          /* Normal action buttons */
          <div className="flex flex-wrap gap-2">
            {/* Update remaining — only for active coupons with an amount */}
            {hasAmount && isActive && (
              <button
                onClick={() => setEditingAmount(true)}
                disabled={anyPending}
                className={`${btnBase} bg-blue-50 text-blue-700 hover:bg-blue-100`}
              >
                עדכן יתרה
              </button>
            )}

            {/* Mark used — active coupons only */}
            {isActive && (
              <button
                onClick={() => markUsedMutation.mutate()}
                disabled={anyPending}
                className={`${btnBase} bg-gray-100 text-gray-700 hover:bg-gray-200`}
              >
                {markUsedMutation.isPending ? '...' : 'מומש'}
              </button>
            )}

            {/* Archive — active or used coupons */}
            {!isArchived && (
              <button
                onClick={() => archiveMutation.mutate()}
                disabled={anyPending}
                className={`${btnBase} bg-gray-100 text-gray-700 hover:bg-gray-200`}
              >
                {archiveMutation.isPending ? '...' : 'ארכיון'}
              </button>
            )}

            {/* Delete — always available */}
            <button
              onClick={handleDelete}
              disabled={anyPending}
              className={`${btnBase} bg-red-50 text-red-600 hover:bg-red-100`}
            >
              {deleteMutation.isPending ? '...' : 'מחק'}
            </button>
          </div>
        )}
      </div>
    </div>
  )
}
