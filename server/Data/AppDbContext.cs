using Microsoft.EntityFrameworkCore;

namespace CouponManager.Api.Data;

/// <summary>
/// Main EF Core database context.
/// DbSet properties will be added here as models are defined.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Example (uncomment when Coupon model is added):
    // public DbSet<Coupon> Coupons { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Entity configuration will go here
    }
}
