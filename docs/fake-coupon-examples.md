# Fake Coupon Examples

These are fictional examples used for development, seeding, and testing only.
Do NOT use real coupon codes, real URLs, or real private voucher links.

---

## Example 1 — Grocery (ILS, fixed date)

```
קיבלת קופון מתנה מרשת סופר-טוב!
שווי הקופון: 75 ש״ח
קוד מימוש: FAKE-GRC-001
תוקף: עד 31.12.2025
לניצול באתר: https://example-fake.il/redeem/FAKE-GRC-001
תנאים: ניתן לשימוש בקנייה מינימלית של 150 ש״ח
```

**Expected parse:**
- merchant: סופר-טוב
- amount: 75
- currency: ILS
- code: FAKE-GRC-001
- expiry: 2025-12-31
- category: food
- onlineRedeemable: true

---

## Example 2 — Fashion (partial use)

```
יש לך קופון שלא מומש במלואו!
מותג: FashionX
שווי מקורי: 200 ש״ח
יתרה: 80 ש״ח
קוד: FAKE-FSH-099
תוקף: 30 יום מיום הרכישה
```

**Expected parse:**
- merchant: FashionX
- originalAmount: 200
- remainingAmount: 80
- currency: ILS
- code: FAKE-FSH-099
- expirationType: rolling_days
- category: fashion
- onlineRedeemable: false (no URL)

---

## Example 3 — Restaurant (no code, low confidence)

```
הצג הודעה זו בקופה וקבל 10% הנחה בארוחה הבאה שלך
בתוקף עד סוף החודש
תנאים: לא כולל שישי ושבת
```

**Expected parse:**
- merchant: (unknown — low confidence)
- amount: 10% (percentage, not fixed)
- code: (none)
- expirationText: "סוף החודש"
- category: food
- confidenceScore: ~0.4
- warnings: ["merchant not identified", "no coupon code found", "expiry date is relative and vague"]

---

## Example 4 — Travel voucher

```
שלום! קיבלת שובר נסיעה מחברת TravelFake.
שווי: 500 ש״ח
תוקף: 01.06.2026
קישור למימוש: https://fake-travel-example.com/voucher/TF-2024-XYZ
קוד אישור: TF-2024-XYZ
```

**Expected parse:**
- provider: TravelFake
- amount: 500
- currency: ILS
- code: TF-2024-XYZ
- expiry: 2026-06-01
- voucherUrl: https://fake-travel-example.com/voucher/TF-2024-XYZ
- category: travel
- onlineRedeemable: true

---

## Usage in Tests

Reference these examples by ID (e.g. `FakeCoupon.Example1`) in xUnit tests.
Never copy real messages — always use these fictional templates.
