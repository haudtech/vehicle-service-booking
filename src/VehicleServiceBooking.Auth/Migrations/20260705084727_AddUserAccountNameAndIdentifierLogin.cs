using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Auth.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAccountNameAndIdentifierLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.Sql(
                @"WITH normalized AS (
                    SELECT
                        ""Id"",
                        COALESCE(
                            NULLIF(
                                REGEXP_REPLACE(LOWER(SPLIT_PART(""Email"", '@', 1)), '[^a-z0-9._-]', '_', 'g'),
                                ''
                            ),
                            'user'
                        ) AS base_name
                    FROM ""Users""
                ),
                ranked AS (
                    SELECT
                        ""Id"",
                        CASE
                            WHEN ROW_NUMBER() OVER (PARTITION BY base_name ORDER BY ""Id"") = 1
                                THEN LEFT(base_name, 50)
                            ELSE LEFT(base_name, 40) || '_' || ROW_NUMBER() OVER (PARTITION BY base_name ORDER BY ""Id"")::TEXT
                        END AS generated_name
                    FROM normalized
                )
                UPDATE ""Users"" u
                SET ""AccountName"" = r.generated_name
                FROM ranked r
                WHERE u.""Id"" = r.""Id"";");

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AccountName",
                table: "Users",
                column: "AccountName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_AccountName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "Users");
        }
    }
}
