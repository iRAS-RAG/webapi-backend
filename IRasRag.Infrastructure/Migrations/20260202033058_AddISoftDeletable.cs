using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddISoftDeletable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "deleted_at", table: "verifications");

            migrationBuilder.DropColumn(name: "deleted_at", table: "user_farms");

            migrationBuilder.DropColumn(name: "deleted_at", table: "species_thresholds");

            migrationBuilder.DropColumn(name: "deleted_at", table: "species_stage_configs");

            migrationBuilder.DropColumn(name: "deleted_at", table: "species");

            migrationBuilder.DropColumn(name: "deleted_at", table: "sensor_types");

            migrationBuilder.DropColumn(name: "deleted_at", table: "sensor_logs");

            migrationBuilder.DropColumn(name: "deleted_at", table: "roles");

            migrationBuilder.DropColumn(name: "deleted_at", table: "refresh_tokens");

            migrationBuilder.DropColumn(name: "deleted_at", table: "recommendations");

            migrationBuilder.DropColumn(name: "deleted_at", table: "mortality_logs");

            migrationBuilder.DropColumn(name: "deleted_at", table: "job_types");

            migrationBuilder.DropColumn(name: "deleted_at", table: "job_control_mappings");

            migrationBuilder.DropColumn(name: "deleted_at", table: "growth_stages");

            migrationBuilder.DropColumn(name: "deleted_at", table: "feeding_logs");

            migrationBuilder.DropColumn(name: "deleted_at", table: "feed_types");

            migrationBuilder.DropColumn(name: "deleted_at", table: "farming_batches");

            migrationBuilder.DropColumn(name: "deleted_at", table: "documents");

            migrationBuilder.DropColumn(name: "deleted_at", table: "corrective_actions");

            migrationBuilder.DropColumn(name: "deleted_at", table: "control_device_types");

            migrationBuilder.DropColumn(name: "deleted_at", table: "alerts");

            migrationBuilder.InsertData(
                table: "species_thresholds",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "growth_stage_id",
                    "max_value",
                    "min_value",
                    "modified_at",
                    "sensor_type_id",
                    "species_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000501"),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        30f,
                        26f,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000502"),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        8f,
                        6.5f,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000503"),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        29f,
                        25f,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000504"),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        8.5f,
                        6.5f,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_growth_stages_name",
                table: "growth_stages",
                column: "name",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_growth_stages_name", table: "growth_stages");

            migrationBuilder.DeleteData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000501")
            );

            migrationBuilder.DeleteData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000502")
            );

            migrationBuilder.DeleteData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000503")
            );

            migrationBuilder.DeleteData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000504")
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "verifications",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "user_farms",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "species_thresholds",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "species_stage_configs",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "species",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "sensor_types",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "sensor_logs",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "roles",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "refresh_tokens",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "recommendations",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "mortality_logs",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "job_types",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "job_control_mappings",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "growth_stages",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "feeding_logs",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "feed_types",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "farming_batches",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "documents",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "corrective_actions",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "control_device_types",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "alerts",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.UpdateData(
                table: "feed_types",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000201"),
                column: "deleted_at",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "feed_types",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000202"),
                column: "deleted_at",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "growth_stages",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                column: "deleted_at",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "growth_stages",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                column: "deleted_at",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                column: "deleted_at",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                column: "deleted_at",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                column: "deleted_at",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                column: "deleted_at",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "species",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                column: "deleted_at",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "species_stage_configs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000601"),
                column: "deleted_at",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "species_stage_configs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
                column: "deleted_at",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "user_farms",
                keyColumn: "id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000001"),
                column: "deleted_at",
                value: null
            );
        }
    }
}
