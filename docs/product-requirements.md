# Product Requirements — Coupon Manager

## MVP Goal

Allow a user to paste a raw SMS or message text that contains a coupon or voucher, have the backend extract structured data from it, and let the user review, edit, save, search, filter, and manage their coupons.

## MVP Features

### Coupon Parsing (Core)
- User pastes raw text on the Add Coupon screen
- Backend parses text with an internal rule-based engine
- Returns structured coupon data
- Parser returns a confidence score and optional warnings

### Coupon Management
- View all saved coupons (list)
- View coupon detail
- Edit coupon fields manually after parsing
- Save coupon to database
- Delete a coupon

### Search & Filter
- Full-text search across coupons
- Filter by category
- Filter by expiration status (active / expired / expiring soon)

### Dashboard
- Summary stats: total coupons, expiring soon, by category
- Backend health status

## Out of Scope for MVP
- Authentication / user accounts
- External AI / LLM APIs
- Sharing coupons
- Push notifications
- Multi-language support beyond Hebrew/RTL

## Future Deployment
- **Frontend**: Vercel or Netlify
- **Backend**: Render, Azure App Service, or Fly.io
- **Database**: Supabase Postgres or Neon Postgres (replacing SQLite)
