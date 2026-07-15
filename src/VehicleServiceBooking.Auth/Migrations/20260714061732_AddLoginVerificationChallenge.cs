using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Auth.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginVerificationChallenge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LoginVerificationChallengeId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoginVerificationCodeAttempts",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LoginVerificationCodeExpiresAtUtc",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginVerificationCodeHash",
                table: "Users",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginVerificationChallengeId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LoginVerificationCodeAttempts",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LoginVerificationCodeExpiresAtUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LoginVerificationCodeHash",
                table: "Users");
        }
    }
}
