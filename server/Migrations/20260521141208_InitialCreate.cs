using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CouponManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RawText = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Provider = table.Column<string>(type: "TEXT", nullable: true),
                    MerchantName = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedMerchantName = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<int>(type: "INTEGER", nullable: false),
                    OriginalAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    RemainingAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: true),
                    CouponCode = table.Column<string>(type: "TEXT", nullable: true),
                    Numerator = table.Column<string>(type: "TEXT", nullable: true),
                    VoucherUrl = table.Column<string>(type: "TEXT", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpirationText = table.Column<string>(type: "TEXT", nullable: true),
                    ExpirationType = table.Column<int>(type: "INTEGER", nullable: false),
                    OnlineRedeemable = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Confidence = table.Column<double>(type: "REAL", nullable: false),
                    ConditionsText = table.Column<string>(type: "TEXT", nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");
        }
    }
}
