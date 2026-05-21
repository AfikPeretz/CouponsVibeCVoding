# Coupon Manager — CLAUDE.md

## Project Overview

A coupon management web app built for learning vibe coding. Users paste raw SMS/message text containing coupons or vouchers. The backend analyzes the text using an internal rule-based coupon extraction engine and returns structured coupon data. Users can review, edit, save, search, filter, and manage coupons.

## Repository Structure

```
/client   — React + Vite + TypeScript frontend
/server   — ASP.NET Core Web API backend (C#)
/docs     — Documentation
```

## Tech Stack

### Frontend (`/client`)
- React
- Vite
- TypeScript
- React Router
- TanStack Query
- Tailwind CSS
- RTL-friendly UI
- Mobile-first design

### Backend (`/server`)
- ASP.NET Core Web API (C#)
- Entity Framework Core
- SQLite (local development)
- PostgreSQL (production, later)
- Swagger / OpenAPI
- xUnit tests

### Deployment (future)
- Frontend: Vercel or Netlify
- Backend: Render, Azure App Service, or Fly.io
- Database: Supabase Postgres or Neon Postgres

## Hard Rules

- Do NOT use Next.js.
- Do NOT add authentication yet.
- Do NOT add external AI APIs yet.
- Do NOT train or integrate a real LLM yet.
- Do NOT commit real coupon codes or private voucher URLs — use fake examples only.
- Keep code simple, readable, and easy to debug.
- Business logic must NOT live inside controllers — use dedicated services.
- Keep the coupon parser split into small, independently testable services.

## Workflow for Large Changes

Before implementing any large change:
1. Explain the plan.
2. List all files that will be created or modified.
3. Implement the change.
4. Run build and/or tests if possible.
5. Summarize what changed.

## Coupon Parser — Target Fields

The rule-based parser should eventually extract:

| Field | Description |
|---|---|
| `provider` | Service or platform that issued the coupon |
| `merchantName` | Raw merchant name from text |
| `normalizedMerchantName` | Cleaned/canonical merchant name |
| `category` | Category (e.g., food, travel, fashion) |
| `originalAmount` | Face value of the coupon |
| `remainingAmount` | Remaining usable value |
| `currency` | Currency code (e.g., ILS, USD) |
| `couponCode` | Redemption code |
| `numerator` | e.g., "1 of 3 uses" |
| `voucherUrl` | Redemption URL |
| `expirationDate` | Parsed expiration date |
| `expirationText` | Raw expiration string from text |
| `expirationType` | e.g., fixed date, rolling days |
| `onlineRedeemable` | Whether usable online |
| `conditionsText` | Raw conditions string |
| `confidenceScore` | Parser confidence (0.0–1.0) |
| `warnings` | List of parser warnings or ambiguities |
