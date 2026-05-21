# Coupon Parser — Rule Design

## Overview

The coupon parser is a rule-based text extraction engine written in C#.
It must NOT use any external AI or LLM APIs.
Each field is extracted by a dedicated, independently testable extractor class.

## Target Fields

| Field | Extractor class | Notes |
|---|---|---|
| `provider` | `ProviderExtractor` | Sending platform (e.g. "Multipass", "Cello") |
| `merchantName` | `MerchantExtractor` | Raw merchant as found in text |
| `normalizedMerchantName` | `MerchantNormalizer` | Canonical name (e.g. "מגה" → "Mega") |
| `category` | `CategoryClassifier` | food, fashion, travel, health, other |
| `originalAmount` | `AmountExtractor` | Face value (e.g. 50) |
| `remainingAmount` | `AmountExtractor` | Remaining value if partial use indicated |
| `currency` | `CurrencyExtractor` | ILS, USD, etc. |
| `couponCode` | `CouponCodeExtractor` | Alphanumeric code |
| `numerator` | `NumeratorExtractor` | "1 of 3 uses" style fields |
| `voucherUrl` | `VoucherUrlExtractor` | Redemption URL |
| `expirationDate` | `ExpirationExtractor` | Parsed DateTime |
| `expirationText` | `ExpirationExtractor` | Raw string (e.g. "עד 31.12.25") |
| `expirationType` | `ExpirationTypeClassifier` | fixed_date, rolling_days, unknown |
| `onlineRedeemable` | `RedemptionClassifier` | bool |
| `conditionsText` | `ConditionsExtractor` | Raw conditions paragraph |
| `confidenceScore` | `ConfidenceCalculator` | 0.0–1.0 |
| `warnings` | `ConfidenceCalculator` | List of string warnings |

## Extraction Strategy

Each extractor receives the normalized input text and returns a typed result.
The orchestrator (`CouponParserService`) runs all extractors and assembles the DTO.

### Merchant extraction
- Match known merchant names list (keyword dictionary)
- Normalize to canonical name
- Flag low-confidence if not matched

### Amount extraction
- Regex: match currency symbols or "ש״ח" / "שקל" / "NIS" followed by/preceded by a number
- Support decimal values

### Coupon code extraction
- Regex: uppercase alphanumeric strings, 4–20 characters
- Context clues: "קוד", "code", "קופון"

### Expiration extraction
- Date formats: `DD.MM.YY`, `DD/MM/YYYY`, Hebrew date patterns
- Relative: "30 יום", "חודש הבא"

### URL extraction
- Regex: standard URL patterns
- Filter out known tracker domains (not coupon redemption links)

## Confidence Scoring

Each extractor reports a local confidence.
The `ConfidenceCalculator` aggregates into a single score:
- Start at 1.0
- Deduct for each missing critical field (merchant, amount, expiry)
- Add warnings for each deduction

## Testing

Every extractor must have xUnit tests.
Use fake coupon text only (see `fake-coupon-examples.md`).
