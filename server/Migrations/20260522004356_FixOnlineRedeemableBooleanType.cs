using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CouponManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixOnlineRedeemableBooleanType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(
                    @"ALTER TABLE ""Coupons""
                      ALTER COLUMN ""OnlineRedeemable"" TYPE boolean
                      USING (""OnlineRedeemable"" <> 0);");
            }
            // SQLite: no-op. INTEGER already stores bool correctly via Microsoft.Data.Sqlite.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(
                    @"ALTER TABLE ""Coupons""
                      ALTER COLUMN ""OnlineRedeemable"" TYPE integer
                      USING (CASE WHEN ""OnlineRedeemable"" THEN 1 ELSE 0 END);");
            }
        }
    }
}
