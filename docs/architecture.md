# Architecture — Coupon Manager

## Overview

```
/client   React + Vite + TypeScript  →  served by Vercel / Netlify
/server   ASP.NET Core Web API (C#)  →  hosted on Render / Azure / Fly.io
/docs     Project documentation
```

## Client

- **Framework**: React 19 + Vite + TypeScript
- **Routing**: React Router v7
- **Data fetching**: TanStack Query v5
- **Styling**: Tailwind CSS v4 (RTL-first, mobile-first)
- **API communication**: fetch via a thin `apiFetch` wrapper in `src/api/`

### Client folder structure

```
src/
  api/            # API helpers (one file per resource)
  components/     # Shared UI components (Layout, NavBar, ...)
  pages/          # One file per route page
  main.tsx        # Entry point — BrowserRouter + QueryClientProvider
  App.tsx         # Route definitions
```

## Server

- **Framework**: ASP.NET Core 8 Web API (controller-based)
- **ORM**: Entity Framework Core 8
- **Database**: SQLite (local dev) → PostgreSQL (production)
- **API docs**: Swagger / OpenAPI via Swashbuckle

### Server folder structure

```
Controllers/                          # Thin HTTP handlers, no business logic
Data/                                 # AppDbContext and EF migrations
Models/                               # EF Core entity classes
DTOs/                                 # Request / response shapes
Services/                             # Business logic services
  CouponParsing/                      # Rule-based parser engine
    Extractors/                       # One extractor per field
    Classifiers/                      # Category and type classifiers
    Confidence/                       # Confidence scoring
Enums/                                # Shared enumerations
```

### Architecture principle

> Controllers are thin. All business logic lives in Services.

## Data Flow

```
User pastes text
  → POST /api/parse (body: { rawText })
    → ParseController
      → CouponParserService
        → Extractors (merchant, amount, code, expiry, url, ...)
        → Classifiers (category, expiration type)
        → ConfidenceCalculator
      ← ParsedCouponDto
    ← 200 OK { coupon: ParsedCouponDto }
  ← UI shows result for review
User confirms
  → POST /api/coupons (save to DB)
```

## Local Development Ports

| Service  | Port |
|----------|------|
| Frontend | 5173 |
| Backend  | 5000 |
