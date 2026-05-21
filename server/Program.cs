using Microsoft.EntityFrameworkCore;
using CouponManager.Api.Data;
using CouponManager.Api.Services.CouponParsing;

var builder = WebApplication.CreateBuilder(args);

// ── Database provider ────────────────────────────────────────────────────────
// Set DATABASE_PROVIDER=postgres in production to use PostgreSQL.
// Defaults to SQLite for local development.
var dbProvider = builder.Configuration["DATABASE_PROVIDER"] ?? "sqlite";

if (dbProvider.Equals("postgres", StringComparison.OrdinalIgnoreCase))
{
    // Npgsql 6+ requires DateTimeKind.Utc. Enable legacy behavior as insurance
    // in case any DateTime values arrive without explicit UTC kind.
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "CONNECTION STRING 'DefaultConnection' is required when DATABASE_PROVIDER=postgres. " +
                "Set ConnectionStrings__DefaultConnection as an environment variable.")));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=coupon-manager.db"));
}

// ── Controllers ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger / OpenAPI ────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Coupon Manager API", Version = "v1" });
});

// ── Coupon parser ────────────────────────────────────────────────────────────
builder.Services.AddScoped<ICouponParserService, CouponParserService>();

// ── CORS ─────────────────────────────────────────────────────────────────────
// In Development: allow the Vite dev server.
// In Production: allow origins listed in ALLOWED_ORIGINS (comma-separated).
//   Example: ALLOWED_ORIGINS=https://my-app.vercel.app,https://custom-domain.com
var allowedOrigins = (builder.Configuration["ALLOWED_ORIGINS"] ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AppCors", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        // If ALLOWED_ORIGINS is not set in production, no origins are allowed.
        // This is intentional — configure it after the Vercel URL is known.
    });
});

// ── Build ────────────────────────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Coupon Manager API v1");
    });
}

app.UseCors("AppCors");
app.MapControllers();

app.Run();
