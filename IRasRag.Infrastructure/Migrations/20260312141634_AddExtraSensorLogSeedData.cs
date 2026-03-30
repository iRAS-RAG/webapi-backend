using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExtraSensorLogSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001401"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[]
                {
                    new DateTime(2026, 1, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    27.800000000000001,
                    "{\"temperature\": 27.8, \"unit\": \"C\"}",
                }
            );

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001402"),
                columns: new[] { "created_at", "data", "data_json", "is_warning" },
                values: new object[]
                {
                    new DateTime(2026, 1, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                    28.199999999999999,
                    "{\"temperature\": 28.2, \"unit\": \"C\"}",
                    false,
                }
            );

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001403"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[]
                {
                    new DateTime(2026, 1, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    7.0999999999999996,
                    "{\"ph\": 7.1}",
                }
            );

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001404"),
                columns: new[] { "created_at", "data", "data_json", "sensor_id" },
                values: new object[]
                {
                    new DateTime(2026, 1, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                    28.600000000000001,
                    "{\"temperature\": 28.6, \"unit\": \"C\"}",
                    new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                }
            );

            migrationBuilder.InsertData(
                table: "sensor_logs",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "data",
                    "data_json",
                    "is_warning",
                    "modified_at",
                    "sensor_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001405"),
                        new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc),
                        29.399999999999999,
                        "{\"temperature\": 29.4, \"unit\": \"C\"}",
                        false,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001406"),
                        new DateTime(2026, 1, 1, 13, 0, 0, 0, DateTimeKind.Utc),
                        31.199999999999999,
                        "{\"temperature\": 31.2, \"unit\": \"C\"}",
                        true,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001407"),
                        new DateTime(2026, 1, 1, 17, 0, 0, 0, DateTimeKind.Utc),
                        30.5,
                        "{\"temperature\": 30.5, \"unit\": \"C\"}",
                        false,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001408"),
                        new DateTime(2026, 1, 1, 21, 0, 0, 0, DateTimeKind.Utc),
                        29.100000000000001,
                        "{\"temperature\": 29.1, \"unit\": \"C\"}",
                        false,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001409"),
                        new DateTime(2026, 1, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                        7.2999999999999998,
                        "{\"ph\": 7.3}",
                        false,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001410"),
                        new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc),
                        7.5,
                        "{\"ph\": 7.5}",
                        false,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001411"),
                        new DateTime(2026, 1, 1, 13, 0, 0, 0, DateTimeKind.Utc),
                        7.4000000000000004,
                        "{\"ph\": 7.4}",
                        false,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001412"),
                        new DateTime(2026, 1, 1, 17, 0, 0, 0, DateTimeKind.Utc),
                        7.5999999999999996,
                        "{\"ph\": 7.6}",
                        false,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001413"),
                        new DateTime(2026, 1, 1, 21, 0, 0, 0, DateTimeKind.Utc),
                        7.2000000000000002,
                        "{\"ph\": 7.2}",
                        false,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001414"),
                        new DateTime(2026, 1, 1, 21, 0, 0, 0, DateTimeKind.Utc),
                        6.7999999999999998,
                        "{\"dissolvedOxygen\": 6.8, \"unit\": \"mg/L\"}",
                        false,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001304"),
                    },
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001405")
            );

            migrationBuilder.DeleteData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001406")
            );

            migrationBuilder.DeleteData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001407")
            );

            migrationBuilder.DeleteData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001408")
            );

            migrationBuilder.DeleteData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001409")
            );

            migrationBuilder.DeleteData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001410")
            );

            migrationBuilder.DeleteData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001411")
            );

            migrationBuilder.DeleteData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001412")
            );

            migrationBuilder.DeleteData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001413")
            );

            migrationBuilder.DeleteData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001414")
            );

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001401"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[]
                {
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    28.5,
                    "{\"temperature\": 28.5, \"unit\": \"C\"}",
                }
            );

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001402"),
                columns: new[] { "created_at", "data", "data_json", "is_warning" },
                values: new object[]
                {
                    new DateTime(2024, 1, 1, 0, 30, 0, 0, DateTimeKind.Utc),
                    31.199999999999999,
                    "{\"temperature\": 31.2, \"unit\": \"C\"}",
                    true,
                }
            );

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001403"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[]
                {
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    7.2000000000000002,
                    "{\"ph\": 7.2}",
                }
            );

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001404"),
                columns: new[] { "created_at", "data", "data_json", "sensor_id" },
                values: new object[]
                {
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    6.7999999999999998,
                    "{\"dissolvedOxygen\": 6.8, \"unit\": \"mg/L\"}",
                    new Guid("aaaaaaaa-0000-0000-0000-000000001304"),
                }
            );
        }
    }
}
