# Deployment Guide

Stack: Render (backend Docker) + Vercel (frontend) + Supabase (PostgreSQL)

---

## 1. Supabase — Create the production database

1. Go to https://supabase.com and sign in.
2. Create a new project (choose a region close to your users).
3. Wait for the project to provision (~1 minute).
4. Go to **Project Settings → Database → Connection string → URI**.
5. Copy the connection string. It looks like:

```
postgresql://postgres.xxxx:your-password@aws-0-eu-central-1.pooler.supabase.com:6543/postgres
```

6. For EF Core / Npgsql, convert it to the key=value format:

```
Host=aws-0-eu-central-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.xxxx;Password=your-password;SSL Mode=Require;Trust Server Certificate=true
```

> **Note:** Use port `6543` (PgBouncer/pooler) for the app, and port `5432` (direct) for running migrations.

---

## 2. Apply EF Core migrations to Supabase

Run this from the `server/` directory on your local machine.
Replace the connection string with your Supabase **direct** connection (port 5432, not pooler).

**On bash/macOS/Linux:**
```bash
cd server
DATABASE_PROVIDER=postgres \
ConnectionStrings__DefaultConnection="Host=db.xxxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=your-password;SSL Mode=Require;Trust Server Certificate=true" \
dotnet ef database update
```

**On Windows PowerShell:**
```powershell
cd server
$env:DATABASE_PROVIDER = "postgres"
$env:ConnectionStrings__DefaultConnection = "Host=db.xxxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=your-password;SSL Mode=Require;Trust Server Certificate=true"
dotnet ef database update
```

You should see `Applying migration '20260521141208_InitialCreate'` in the output.

> **Why manual migration?** Running migrations automatically on startup is risky in production:
> it can cause data loss on schema changes, race conditions in multi-instance deployments,
> and makes rollbacks harder. Manual migration gives you full control.

---

## 3. Deploy backend to Render

### Create the Render service

1. Go to https://render.com and sign in.
2. Click **New → Web Service**.
3. Connect your GitHub repository.
4. Configure:
   - **Name:** `coupon-manager-api` (or any name)
   - **Root Directory:** `server`
   - **Runtime:** Docker
   - **Dockerfile Path:** `./Dockerfile` (relative to root directory)
   - **Port:** `8080`

### Set environment variables on Render

Go to **Environment → Environment Variables** and add:

| Key | Value |
|-----|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `DATABASE_PROVIDER` | `postgres` |
| `ConnectionStrings__DefaultConnection` | `Host=aws-0-...pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.xxxx;Password=...;SSL Mode=Require;Trust Server Certificate=true` |
| `ALLOWED_ORIGINS` | *(leave blank for now — fill in after Vercel deployment)* |

> Use the **pooler** connection string (port 6543) for the running app, not the direct connection.

### Deploy

Click **Create Web Service**. Render will build the Docker image and deploy.

After deploy, note your Render URL: `https://coupon-manager-api.onrender.com`

Verify it's alive: `https://coupon-manager-api.onrender.com/api/health`

---

## 4. Deploy frontend to Vercel

1. Go to https://vercel.com and sign in.
2. Click **Add New → Project**.
3. Import your GitHub repository.
4. Configure:
   - **Framework Preset:** Vite
   - **Root Directory:** `client`
   - **Build Command:** `npm run build`
   - **Output Directory:** `dist`

### Set environment variables on Vercel

Go to **Settings → Environment Variables** and add:

| Key | Value |
|-----|-------|
| `VITE_API_BASE_URL` | `https://coupon-manager-api.onrender.com` |

> Do not include a trailing slash.

Click **Deploy**.

After deploy, note your Vercel URL: `https://your-app.vercel.app`

---

## 5. Update CORS on Render

Now that the Vercel URL is known:

1. Go to Render → your service → **Environment**.
2. Set:
   - `ALLOWED_ORIGINS` → `https://your-app.vercel.app`
3. Click **Save Changes**. Render will redeploy automatically.

If you add a custom domain to Vercel later, add it to `ALLOWED_ORIGINS` as well (comma-separated):
```
ALLOWED_ORIGINS=https://your-app.vercel.app,https://your-custom-domain.com
```

---

## 6. Verify end-to-end

1. Open `https://your-app.vercel.app`
2. Go to Add Coupon and paste a fake coupon text — the analyze call should work
3. Save a coupon — it should appear in My Coupons
4. Search, categories, and actions should all work

---

## Local development (unchanged)

```bash
# Backend
cd server
dotnet run
# Runs on http://localhost:5000, uses SQLite (coupon-manager.db)

# Frontend
cd client
npm run dev
# Runs on http://localhost:5173, reads VITE_API_BASE_URL from .env
```

---

## Future migrations

When you add new migrations:

```bash
# Generate migration (local)
cd server
dotnet ef migrations add YourMigrationName

# Apply to production Postgres
DATABASE_PROVIDER=postgres \
ConnectionStrings__DefaultConnection="Host=db.xxxx.supabase.co;Port=5432;..." \
dotnet ef database update
```

---

## Render free tier note

Render's free tier spins down the service after 15 minutes of inactivity.
The first request after sleep takes ~30 seconds (cold start).
To avoid this, upgrade to a paid plan or use Render's cron job to ping `/api/health` every 10 minutes.
