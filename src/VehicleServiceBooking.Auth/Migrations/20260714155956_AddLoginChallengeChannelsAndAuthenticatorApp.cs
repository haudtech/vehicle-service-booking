using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleServiceBooking.Auth.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginChallengeChannelsAndAuthenticatorApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthenticatorAppSecret",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAuthenticatorAppEnabled",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LoginVerificationChannel",
                table: "Users",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "email_otp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthenticatorAppSecret",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsAuthenticatorAppEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LoginVerificationChannel",
                table: "Users");
        }
    }
}
