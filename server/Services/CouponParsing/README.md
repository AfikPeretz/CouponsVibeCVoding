# CouponParsing

The rule-based coupon extraction engine.

Orchestrator entry point: `CouponParserService.cs`

Sub-folders:
- `Extractors/` — field extraction logic (merchant, amount, code, expiry, etc.)
- `Classifiers/` — category and type classification
- `Confidence/` — confidence scoring logic
