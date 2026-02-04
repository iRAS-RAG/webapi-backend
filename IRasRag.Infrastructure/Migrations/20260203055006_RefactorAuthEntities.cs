using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAuthEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_users_user_name", table: "users");

            migrationBuilder.DropColumn(name: "is_verified", table: "users");

            migrationBuilder.DropColumn(name: "user_name", table: "users");

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "verifications",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_revoked",
                table: "refresh_tokens",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                column: "name",
                value: "Supervisor"
            );

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "created_at", "modified_at", "name" },
                values: new object[]
                {
                    new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    null,
                    "Operator",
                }
            );

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                column: "email",
                value: "supervisor@example.com"
            );

            migrationBuilder.InsertData(
                table: "users",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "deleted_at",
                    "email",
                    "first_name",
                    "is_deleted",
                    "last_name",
                    "modified_at",
                    "password_hash",
                    "role_id",
                },
                values: new object[]
                {
                    new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    null,
                    "operator@example.com",
                    "",
                    false,
                    "",
                    null,
                    "$2a$11$TjsTmXlpjjTVPajZiLxCV.XPuTPgCgphg7sfC9Fs/YwSA4M4IYqYu",
                    new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000003")
            );

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000003")
            );

            migrationBuilder.DropColumn(name: "type", table: "verifications");

            migrationBuilder.DropColumn(name: "is_revoked", table: "refresh_tokens");

            migrationBuilder.AddColumn<bool>(
                name: "is_verified",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<string>(
                name: "user_name",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                column: "name",
                value: "User"
            );

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                columns: new[] { "is_verified", "user_name" },
                values: new object[] { true, "admin" }
            );

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                columns: new[] { "email", "is_verified", "user_name" },
                values: new object[] { "user@example.com", true, "user" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_user_name",
                table: "users",
                column: "user_name",
                unique: true
            );
        }
    }
}
