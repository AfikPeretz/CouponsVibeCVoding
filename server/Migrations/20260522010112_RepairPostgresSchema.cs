using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CouponManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class RepairPostgresSchema : Migration
    {
        // Repairs the Coupons table on PostgreSQL so column types match the C# model.
        // The initial migration was scaffolded against SQLite and baked SQLite-flavored
        // type strings ("TEXT", "REAL", "INTEGER") into the migration, which Postgres
        // honored literally. This migration is idempotent: each ALTER is gated on the
        // current information_schema.columns.data_type so re-runs are no-ops.
        // SQLite branch is a no-op — local development is untouched.

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider != "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                return;
            }

            // DateTime (NOT NULL): CreatedAt, UpdatedAt — text → timestamp without time zone
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'CreatedAt') = 'text' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone
                            USING ""CreatedAt""::timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'UpdatedAt') = 'text' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone
                            USING ""UpdatedAt""::timestamp without time zone;
                    END IF;
                END $$;
            ");

            // DateTime? (NULLABLE): ExpirationDate, ReceivedAt — text → timestamp without time zone
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'ExpirationDate') = 'text' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""ExpirationDate"" TYPE timestamp without time zone
                            USING NULLIF(""ExpirationDate"", '')::timestamp without time zone;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'ReceivedAt') = 'text' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""ReceivedAt"" TYPE timestamp without time zone
                            USING NULLIF(""ReceivedAt"", '')::timestamp without time zone;
                    END IF;
                END $$;
            ");

            // decimal? (NULLABLE): OriginalAmount, RemainingAmount — text → numeric
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'OriginalAmount') = 'text' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""OriginalAmount"" TYPE numeric
                            USING NULLIF(""OriginalAmount"", '')::numeric;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'RemainingAmount') = 'text' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""RemainingAmount"" TYPE numeric
                            USING NULLIF(""RemainingAmount"", '')::numeric;
                    END IF;
                END $$;
            ");

            // double (NOT NULL): Confidence — real → double precision (widening cast)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'Confidence') = 'real' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""Confidence"" TYPE double precision
                            USING ""Confidence""::double precision;
                    END IF;
                END $$;
            ");

            // bool (NOT NULL): OnlineRedeemable — integer → boolean
            // (Also fixed by an earlier migration; this guarded re-application is a no-op
            // when the column is already boolean.)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'OnlineRedeemable') = 'integer' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""OnlineRedeemable"" TYPE boolean
                            USING (""OnlineRedeemable"" <> 0);
                    END IF;
                END $$;
            ");

            // int PK (NOT NULL, value-generated): Id — add IDENTITY if missing
            // (Also fixed by an earlier migration; this guarded re-application is a no-op
            // when the column is already an identity.)
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    is_identity text;
                    next_id bigint;
                BEGIN
                    SELECT c.is_identity INTO is_identity
                    FROM information_schema.columns c
                    WHERE c.table_schema = 'public' AND c.table_name = 'Coupons' AND c.column_name = 'Id';

                    IF is_identity = 'NO' THEN
                        SELECT COALESCE(MAX(""Id""), 0) + 1 INTO next_id FROM ""Coupons"";
                        EXECUTE format(
                            'ALTER TABLE ""Coupons"" ALTER COLUMN ""Id"" ADD GENERATED BY DEFAULT AS IDENTITY (START WITH %s)',
                            next_id
                        );
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider != "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                return;
            }

            // Reverse: restore the SQLite-flavored shapes the initial migration produced on Postgres.
            // Each ALTER is gated so re-running Down is safe.

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT is_identity FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'Id') = 'YES' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'OnlineRedeemable') = 'boolean' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""OnlineRedeemable"" TYPE integer
                            USING (CASE WHEN ""OnlineRedeemable"" THEN 1 ELSE 0 END);
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'Confidence') = 'double precision' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""Confidence"" TYPE real USING ""Confidence""::real;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'OriginalAmount') = 'numeric' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""OriginalAmount"" TYPE text USING ""OriginalAmount""::text;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'RemainingAmount') = 'numeric' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""RemainingAmount"" TYPE text USING ""RemainingAmount""::text;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'ExpirationDate') = 'timestamp without time zone' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""ExpirationDate"" TYPE text USING to_char(""ExpirationDate"", 'YYYY-MM-DD HH24:MI:SS.US');
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'ReceivedAt') = 'timestamp without time zone' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""ReceivedAt"" TYPE text USING to_char(""ReceivedAt"", 'YYYY-MM-DD HH24:MI:SS.US');
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'CreatedAt') = 'timestamp without time zone' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""CreatedAt"" TYPE text USING to_char(""CreatedAt"", 'YYYY-MM-DD HH24:MI:SS.US');
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF (SELECT data_type FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'Coupons' AND column_name = 'UpdatedAt') = 'timestamp without time zone' THEN
                        ALTER TABLE ""Coupons"" ALTER COLUMN ""UpdatedAt"" TYPE text USING to_char(""UpdatedAt"", 'YYYY-MM-DD HH24:MI:SS.US');
                    END IF;
                END $$;
            ");
        }
    }
}
