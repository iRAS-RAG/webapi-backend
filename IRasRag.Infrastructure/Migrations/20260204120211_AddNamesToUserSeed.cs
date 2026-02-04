using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNamesToUserSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                columns: new[] { "first_name", "last_name" },
                values: new object[] { "Văn A", "Nguyễn" }
            );

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                columns: new[] { "first_name", "last_name" },
                values: new object[] { "Thị B", "Trần" }
            );

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                columns: new[] { "first_name", "last_name" },
                values: new object[] { "Văn C", "Lê" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                columns: new[] { "first_name", "last_name" },
                values: new object[] { "", "" }
            );

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                columns: new[] { "first_name", "last_name" },
                values: new object[] { "", "" }
            );

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                columns: new[] { "first_name", "last_name" },
                values: new object[] { "", "" }
            );
        }
    }
}
