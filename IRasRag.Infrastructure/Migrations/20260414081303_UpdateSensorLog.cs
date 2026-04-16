using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSensorLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_alerts_sensor_logs_sensor_log_id",
                table: "alerts");

            migrationBuilder.DropIndex(
                name: "ix_sensor_logs_sensor_id_created_at",
                table: "sensor_logs");

            migrationBuilder.DropColumn(
                name: "data_json",
                table: "sensor_logs");

            migrationBuilder.RenameColumn(
                name: "is_warning",
                table: "sensor_logs",
                newName: "has_warning");

            migrationBuilder.RenameColumn(
                name: "data",
                table: "sensor_logs",
                newName: "min");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "alerts",
                newName: "trigger_value");

            migrationBuilder.RenameColumn(
                name: "sensor_log_id",
                table: "alerts",
                newName: "sensor_id");

            migrationBuilder.RenameIndex(
                name: "ix_alerts_sensor_log_id_status",
                table: "alerts",
                newName: "ix_alerts_sensor_id_status");

            migrationBuilder.AddColumn<double>(
                name: "average",
                table: "sensor_logs",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "max",
                table: "sensor_logs",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "period_start",
                table: "sensor_logs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "sample_count",
                table: "sensor_logs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001801"),
                columns: new[] { "resolved_at", "sensor_id", "status" },
                values: new object[] { new DateTime(2024, 1, 15, 18, 0, 0, 0, DateTimeKind.Utc), new Guid("aaaaaaaa-0000-0000-0000-000000001301"), "RESOLVED" });

            migrationBuilder.UpdateData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001802"),
                column: "sensor_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000001302"));

            migrationBuilder.UpdateData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001803"),
                column: "sensor_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000001301"));

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001401"),
                columns: new[] { "average", "created_at", "max", "min", "period_start", "sample_count" },
                values: new object[] { 27.899999999999999, new DateTime(2026, 1, 1, 4, 0, 0, 0, DateTimeKind.Utc), 28.399999999999999, 27.5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8 });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001402"),
                columns: new[] { "average", "created_at", "max", "min", "period_start", "sample_count" },
                values: new object[] { 28.600000000000001, new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc), 29.100000000000001, 28.100000000000001, new DateTime(2026, 1, 1, 4, 0, 0, 0, DateTimeKind.Utc), 8 });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001403"),
                columns: new[] { "average", "created_at", "max", "min", "period_start", "sample_count" },
                values: new object[] { 7.0999999999999996, new DateTime(2026, 1, 1, 4, 0, 0, 0, DateTimeKind.Utc), 7.2000000000000002, 7.0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8 });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001404"),
                columns: new[] { "average", "created_at", "max", "min", "period_start", "sample_count" },
                values: new object[] { 29.800000000000001, new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), 30.399999999999999, 29.100000000000001, new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc), 8 });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001405"),
                columns: new[] { "average", "created_at", "has_warning", "max", "min", "period_start", "sample_count" },
                values: new object[] { 31.100000000000001, new DateTime(2026, 1, 1, 16, 0, 0, 0, DateTimeKind.Utc), true, 31.800000000000001, 30.5, new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), 8 });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001406"),
                columns: new[] { "average", "created_at", "has_warning", "max", "min", "period_start", "sample_count" },
                values: new object[] { 30.300000000000001, new DateTime(2026, 1, 1, 20, 0, 0, 0, DateTimeKind.Utc), false, 30.899999999999999, 29.800000000000001, new DateTime(2026, 1, 1, 16, 0, 0, 0, DateTimeKind.Utc), 8 });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001407"),
                columns: new[] { "average", "created_at", "max", "min", "period_start", "sample_count" },
                values: new object[] { 29.0, new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 29.5, 28.600000000000001, new DateTime(2026, 1, 1, 20, 0, 0, 0, DateTimeKind.Utc), 8 });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001408"),
                columns: new[] { "average", "created_at", "max", "min", "period_start", "sample_count", "sensor_id" },
                values: new object[] { 27.0, new DateTime(2026, 1, 1, 4, 0, 0, 0, DateTimeKind.Utc), 27.399999999999999, 26.800000000000001, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, new Guid("aaaaaaaa-0000-0000-0000-000000001303") });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001409"),
                columns: new[] { "average", "created_at", "max", "min", "period_start", "sample_count" },
                values: new object[] { 7.2999999999999998, new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc), 7.4000000000000004, 7.2000000000000002, new DateTime(2026, 1, 1, 4, 0, 0, 0, DateTimeKind.Utc), 8 });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001410"),
                columns: new[] { "average", "created_at", "max", "min", "period_start", "sample_count" },
                values: new object[] { 7.5, new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), 7.5999999999999996, 7.4000000000000004, new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc), 8 });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001411"),
                columns: new[] { "average", "created_at", "max", "min", "period_start", "sample_count" },
                values: new object[] { 7.4000000000000004, new DateTime(2026, 1, 1, 16, 0, 0, 0, DateTimeKind.Utc), 7.5, 7.2999999999999998, new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), 8 });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001412"),
                columns: new[] { "average", "created_at", "max", "min", "period_start", "sample_count" },
                values: new object[] { 7.5999999999999996, new DateTime(2026, 1, 1, 20, 0, 0, 0, DateTimeKind.Utc), 7.7000000000000002, 7.5, new DateTime(2026, 1, 1, 16, 0, 0, 0, DateTimeKind.Utc), 8 });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001413"),
                columns: new[] { "average", "created_at", "max", "min", "period_start", "sample_count" },
                values: new object[] { 7.2000000000000002, new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 7.2999999999999998, 7.0999999999999996, new DateTime(2026, 1, 1, 20, 0, 0, 0, DateTimeKind.Utc), 8 });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001414"),
                columns: new[] { "average", "created_at", "max", "min", "period_start", "sample_count" },
                values: new object[] { 6.7999999999999998, new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 7.0999999999999996, 6.5, new DateTime(2026, 1, 1, 20, 0, 0, 0, DateTimeKind.Utc), 8 });

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                columns: new[] { "measure_type", "name", "unit_of_measure" },
                values: new object[] { "Tổng chất rắn hòa tan", "TDS", "ppm" });

            migrationBuilder.InsertData(
                table: "sensor_types",
                columns: new[] { "id", "created_at", "measure_type", "modified_at", "name", "unit_of_measure" },
                values: new object[,]
                {
                    { new Guid("eeeeeeee-0000-0000-0000-000000000004"), null, "Lưu lượng", null, "Lưu lượng nước", "L/min" },
                    { new Guid("eeeeeeee-0000-0000-0000-000000000005"), null, "Mức nước", null, "Mực nước", "0/1" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_sensor_logs_sensor_id_period_start",
                table: "sensor_logs",
                columns: new[] { "sensor_id", "period_start" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_alerts_fish_tank_id_farming_batch_id_sensor_type_id",
                table: "alerts",
                columns: new[] { "fish_tank_id", "farming_batch_id", "sensor_type_id" },
                unique: true,
                filter: "\"status\" IN ('OPEN', 'ACKNOWLEDGED') AND \"farming_batch_id\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_alerts_fish_tank_id_sensor_type_id",
                table: "alerts",
                columns: new[] { "fish_tank_id", "sensor_type_id" },
                unique: true,
                filter: "\"status\" IN ('OPEN', 'ACKNOWLEDGED') AND \"farming_batch_id\" IS NULL");

            migrationBuilder.AddForeignKey(
                name: "fk_alerts_sensors_sensor_id",
                table: "alerts",
                column: "sensor_id",
                principalTable: "sensors",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_alerts_sensors_sensor_id",
                table: "alerts");

            migrationBuilder.DropIndex(
                name: "ix_sensor_logs_sensor_id_period_start",
                table: "sensor_logs");

            migrationBuilder.DropIndex(
                name: "ix_alerts_fish_tank_id_farming_batch_id_sensor_type_id",
                table: "alerts");

            migrationBuilder.DropIndex(
                name: "ix_alerts_fish_tank_id_sensor_type_id",
                table: "alerts");

            migrationBuilder.DeleteData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: new Guid("eeeeeeee-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: new Guid("eeeeeeee-0000-0000-0000-000000000005"));

            migrationBuilder.DropColumn(
                name: "average",
                table: "sensor_logs");

            migrationBuilder.DropColumn(
                name: "max",
                table: "sensor_logs");

            migrationBuilder.DropColumn(
                name: "period_start",
                table: "sensor_logs");

            migrationBuilder.DropColumn(
                name: "sample_count",
                table: "sensor_logs");

            migrationBuilder.RenameColumn(
                name: "min",
                table: "sensor_logs",
                newName: "data");

            migrationBuilder.RenameColumn(
                name: "has_warning",
                table: "sensor_logs",
                newName: "is_warning");

            migrationBuilder.RenameColumn(
                name: "trigger_value",
                table: "alerts",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "sensor_id",
                table: "alerts",
                newName: "sensor_log_id");

            migrationBuilder.RenameIndex(
                name: "ix_alerts_sensor_id_status",
                table: "alerts",
                newName: "ix_alerts_sensor_log_id_status");

            migrationBuilder.AddColumn<string>(
                name: "data_json",
                table: "sensor_logs",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.UpdateData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001801"),
                columns: new[] { "resolved_at", "sensor_log_id", "status" },
                values: new object[] { null, new Guid("aaaaaaaa-0000-0000-0000-000000001402"), "OPEN" });

            migrationBuilder.UpdateData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001802"),
                column: "sensor_log_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000001403"));

            migrationBuilder.UpdateData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001803"),
                column: "sensor_log_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000001401"));

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001401"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[] { new DateTime(2026, 1, 1, 1, 0, 0, 0, DateTimeKind.Utc), 27.800000000000001, "{\"temperature\": 27.8, \"unit\": \"C\"}" });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001402"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[] { new DateTime(2026, 1, 1, 2, 0, 0, 0, DateTimeKind.Utc), 28.199999999999999, "{\"temperature\": 28.2, \"unit\": \"C\"}" });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001403"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[] { new DateTime(2026, 1, 1, 1, 0, 0, 0, DateTimeKind.Utc), 7.0999999999999996, "{\"ph\": 7.1}" });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001404"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[] { new DateTime(2026, 1, 1, 5, 0, 0, 0, DateTimeKind.Utc), 28.600000000000001, "{\"temperature\": 28.6, \"unit\": \"C\"}" });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001405"),
                columns: new[] { "created_at", "data", "data_json", "is_warning" },
                values: new object[] { new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc), 29.399999999999999, "{\"temperature\": 29.4, \"unit\": \"C\"}", false });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001406"),
                columns: new[] { "created_at", "data", "data_json", "is_warning" },
                values: new object[] { new DateTime(2026, 1, 1, 13, 0, 0, 0, DateTimeKind.Utc), 31.199999999999999, "{\"temperature\": 31.2, \"unit\": \"C\"}", true });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001407"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[] { new DateTime(2026, 1, 1, 17, 0, 0, 0, DateTimeKind.Utc), 30.5, "{\"temperature\": 30.5, \"unit\": \"C\"}" });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001408"),
                columns: new[] { "created_at", "data", "data_json", "sensor_id" },
                values: new object[] { new DateTime(2026, 1, 1, 21, 0, 0, 0, DateTimeKind.Utc), 29.100000000000001, "{\"temperature\": 29.1, \"unit\": \"C\"}", new Guid("aaaaaaaa-0000-0000-0000-000000001301") });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001409"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[] { new DateTime(2026, 1, 1, 5, 0, 0, 0, DateTimeKind.Utc), 7.2999999999999998, "{\"ph\": 7.3}" });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001410"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[] { new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc), 7.5, "{\"ph\": 7.5}" });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001411"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[] { new DateTime(2026, 1, 1, 13, 0, 0, 0, DateTimeKind.Utc), 7.4000000000000004, "{\"ph\": 7.4}" });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001412"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[] { new DateTime(2026, 1, 1, 17, 0, 0, 0, DateTimeKind.Utc), 7.5999999999999996, "{\"ph\": 7.6}" });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001413"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[] { new DateTime(2026, 1, 1, 21, 0, 0, 0, DateTimeKind.Utc), 7.2000000000000002, "{\"ph\": 7.2}" });

            migrationBuilder.UpdateData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001414"),
                columns: new[] { "created_at", "data", "data_json" },
                values: new object[] { new DateTime(2026, 1, 1, 21, 0, 0, 0, DateTimeKind.Utc), 6.7999999999999998, "{\"dissolvedOxygen\": 6.8, \"unit\": \"mg/L\"}" });

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                columns: new[] { "measure_type", "name", "unit_of_measure" },
                values: new object[] { "Nồng độ oxy", "Oxy hòa tan", "mg/L" });

            migrationBuilder.CreateIndex(
                name: "ix_sensor_logs_sensor_id_created_at",
                table: "sensor_logs",
                columns: new[] { "sensor_id", "created_at" });

            migrationBuilder.AddForeignKey(
                name: "fk_alerts_sensor_logs_sensor_log_id",
                table: "alerts",
                column: "sensor_log_id",
                principalTable: "sensor_logs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
