using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VehicleServiceBooking.Auth.Migrations
{
    /// <inheritdoc />
    public partial class UseIdPrimaryKeysForAuthJunctionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupRoles",
                table: "GroupRoles");

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumns: new[] { "GroupId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c001"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") });

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumns: new[] { "GroupId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c001"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") });

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumns: new[] { "GroupId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011") });

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumns: new[] { "GroupId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012") });

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumns: new[] { "GroupId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") });

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumns: new[] { "GroupId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") });

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumns: new[] { "GroupId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011") });

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumns: new[] { "GroupId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a004"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a006"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a007"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a008"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a009"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a004"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a006"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a007"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a008"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a009"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012") });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupRoles",
                table: "GroupRoles",
                column: "Id");

            migrationBuilder.InsertData(
                table: "GroupRoles",
                columns: new[] { "Id", "CreatedAt", "GroupId", "IsActive", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c001"), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d002"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c001"), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d003"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c002"), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d004"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c002"), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d005"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d006"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d007"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d008"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "IsActive", "PermissionId", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b002"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b003"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a004"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b005"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b006"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a006"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b007"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a007"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b008"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a008"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b009"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a009"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b013"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a004"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b014"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b015"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a006"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b016"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a007"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b017"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a008"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b018"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a009"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_UserId_GroupId",
                table: "UserGroups",
                columns: new[] { "UserId", "GroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoles_GroupId_RoleId",
                table: "GroupRoles",
                columns: new[] { "GroupId", "RoleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups");

            migrationBuilder.DropIndex(
                name: "IX_UserGroups_UserId_GroupId",
                table: "UserGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupRoles",
                table: "GroupRoles");

            migrationBuilder.DropIndex(
                name: "IX_GroupRoles_GroupId_RoleId",
                table: "GroupRoles");

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d001"));

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d002"));

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d003"));

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d004"));

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d005"));

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d006"));

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d007"));

            migrationBuilder.DeleteData(
                table: "GroupRoles",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d008"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b001"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b002"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b003"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b005"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b006"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b007"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b008"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b009"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b011"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b012"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b013"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b014"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b015"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b016"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b017"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b018"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups",
                columns: new[] { "UserId", "GroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupRoles",
                table: "GroupRoles",
                columns: new[] { "GroupId", "RoleId" });

            migrationBuilder.InsertData(
                table: "GroupRoles",
                columns: new[] { "GroupId", "RoleId", "CreatedAt", "Id", "IsActive", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c001"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d001"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c001"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d002"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d003"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d004"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d006"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d008"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d005"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5c003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5d007"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId", "CreatedAt", "Id", "IsActive", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b001"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b002"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a004"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a001"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b003"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b005"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a006"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b006"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a007"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b007"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a008"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b008"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a009"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a010"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b009"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a002"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b011"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a003"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b012"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a004"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a011"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b013"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a005"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b014"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a006"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b015"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a007"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b016"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a008"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b017"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a009"), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5a012"), new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("2d7b6113-2351-4b60-848f-8c3d28f5b018"), true, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }
    }
}
