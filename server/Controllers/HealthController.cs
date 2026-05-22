using CouponManager.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CouponManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _db;

    public HealthController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new { status = "ok" });
    }

    [HttpGet("db-schema")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDbSchema(CancellationToken ct)
    {
        var providerName = _db.Database.ProviderName ?? "unknown";

        bool canConnect;
        try
        {
            canConnect = await _db.Database.CanConnectAsync(ct);
        }
        catch
        {
            canConnect = false;
        }

        // Expected PG types per the C# Coupon model (legacy Npgsql timestamp behavior).
        var expected = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Id"] = "integer",
            ["RawText"] = "text",
            ["Title"] = "text",
            ["Provider"] = "text",
            ["MerchantName"] = "text",
            ["NormalizedMerchantName"] = "text",
            ["Category"] = "integer",
            ["OriginalAmount"] = "numeric",
            ["RemainingAmount"] = "numeric",
            ["Currency"] = "text",
            ["CouponCode"] = "text",
            ["Numerator"] = "text",
            ["VoucherUrl"] = "text",
            ["ExpirationDate"] = "timestamp without time zone",
            ["ExpirationText"] = "text",
            ["ExpirationType"] = "integer",
            ["OnlineRedeemable"] = "boolean",
            ["Status"] = "integer",
            ["Confidence"] = "double precision",
            ["ConditionsText"] = "text",
            ["ReceivedAt"] = "timestamp without time zone",
            ["CreatedAt"] = "timestamp without time zone",
            ["UpdatedAt"] = "timestamp without time zone",
        };

        var isPostgres = providerName.Contains("Npgsql", StringComparison.OrdinalIgnoreCase);

        if (!isPostgres || !canConnect)
        {
            return Ok(new
            {
                provider = providerName,
                canConnect,
                couponsColumns = Array.Empty<object>(),
                warnings = Array.Empty<string>(),
            });
        }

        var columns = new List<ColumnInfo>();
        var warnings = new List<string>();

        var conn = _db.Database.GetDbConnection();
        await conn.OpenAsync(ct);
        try
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT column_name, data_type, is_nullable, is_identity
                FROM information_schema.columns
                WHERE table_schema = 'public' AND table_name = 'Coupons'
                ORDER BY ordinal_position;";

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                var name = reader.GetString(0);
                var dataType = reader.GetString(1);
                var isNullable = reader.GetString(2) == "YES";
                var isIdentity = reader.GetString(3) == "YES";
                columns.Add(new ColumnInfo(name, dataType, isNullable, isIdentity));
            }
        }
        finally
        {
            await conn.CloseAsync();
        }

        foreach (var (name, expectedType) in expected)
        {
            var actual = columns.FirstOrDefault(c => c.Name == name);
            if (actual is null)
            {
                warnings.Add($"Column '{name}' is missing from Coupons table.");
                continue;
            }
            if (!string.Equals(actual.DataType, expectedType, StringComparison.OrdinalIgnoreCase))
            {
                warnings.Add($"Column '{name}' expected '{expectedType}' but is '{actual.DataType}'.");
            }
        }

        var idCol = columns.FirstOrDefault(c => c.Name == "Id");
        if (idCol is { IsIdentity: false })
        {
            warnings.Add("Column 'Id' is not configured as IDENTITY — inserts will fail without an explicit value.");
        }

        return Ok(new
        {
            provider = providerName,
            canConnect,
            couponsColumns = columns,
            warnings,
        });
    }

    private sealed record ColumnInfo(string Name, string DataType, bool IsNullable, bool IsIdentity);
}
