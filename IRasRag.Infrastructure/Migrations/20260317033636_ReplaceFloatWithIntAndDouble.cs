using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceFloatWithIntAndDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "min_value",
                table: "species_thresholds",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real"
            );

            migrationBuilder.AlterColumn<double>(
                name: "max_value",
                table: "species_thresholds",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real"
            );

            migrationBuilder.AlterColumn<double>(
                name: "max_stocking_density",
                table: "species_stage_configs",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<double>(
                name: "amount_per100fish",
                table: "species_stage_configs",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real"
            );

            migrationBuilder.AlterColumn<int>(
                name: "quantity",
                table: "mortality_logs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real"
            );

            migrationBuilder.AlterColumn<double>(
                name: "min_value",
                table: "jobs",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<double>(
                name: "max_value",
                table: "jobs",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<double>(
                name: "radius",
                table: "fish_tanks",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real"
            );

            migrationBuilder.AlterColumn<double>(
                name: "height",
                table: "fish_tanks",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real"
            );

            migrationBuilder.AlterColumn<double>(
                name: "amount",
                table: "feeding_logs",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real"
            );

            migrationBuilder.AlterColumn<double>(
                name: "protein_percentage",
                table: "feed_types",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real"
            );

            migrationBuilder.AlterColumn<int>(
                name: "initial_quantity",
                table: "farming_batches",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real"
            );

            migrationBuilder.AlterColumn<int>(
                name: "current_quantity",
                table: "farming_batches",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real"
            );

            migrationBuilder.AlterColumn<double>(
                name: "value",
                table: "alerts",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real"
            );

            migrationBuilder.UpdateData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001801"),
                column: "value",
                value: 31.199999999999999
            );

            migrationBuilder.UpdateData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001802"),
                column: "value",
                value: 7.2000000000000002
            );

            migrationBuilder.UpdateData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001803"),
                column: "value",
                value: 28.5
            );

            migrationBuilder.UpdateData(
                table: "farming_batches",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                columns: new[] { "current_quantity", "initial_quantity" },
                values: new object[] { 950, 1000 }
            );

            migrationBuilder.UpdateData(
                table: "farming_batches",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001502"),
                columns: new[] { "current_quantity", "initial_quantity" },
                values: new object[] { 0, 800 }
            );

            migrationBuilder.UpdateData(
                table: "feed_types",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000201"),
                column: "protein_percentage",
                value: 45.0
            );

            migrationBuilder.UpdateData(
                table: "feed_types",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000202"),
                column: "protein_percentage",
                value: 38.0
            );

            migrationBuilder.UpdateData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001601"),
                column: "amount",
                value: 5.5
            );

            migrationBuilder.UpdateData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001602"),
                column: "amount",
                value: 5.7999999999999998
            );

            migrationBuilder.UpdateData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001603"),
                column: "amount",
                value: 6.0
            );

            migrationBuilder.UpdateData(
                table: "fish_tanks",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                columns: new[] { "height", "radius" },
                values: new object[] { 0.0, 0.0 }
            );

            migrationBuilder.UpdateData(
                table: "jobs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001001"),
                columns: new[] { "max_value", "min_value" },
                values: new object[] { null, null }
            );

            migrationBuilder.UpdateData(
                table: "jobs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001002"),
                columns: new[] { "max_value", "min_value" },
                values: new object[] { 30.0, null }
            );

            migrationBuilder.UpdateData(
                table: "jobs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001003"),
                columns: new[] { "max_value", "min_value" },
                values: new object[] { null, null }
            );

            migrationBuilder.UpdateData(
                table: "mortality_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001701"),
                column: "quantity",
                value: 30
            );

            migrationBuilder.UpdateData(
                table: "mortality_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001702"),
                column: "quantity",
                value: 20
            );

            migrationBuilder.UpdateData(
                table: "species_stage_configs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000601"),
                columns: new[] { "amount_per100fish", "max_stocking_density" },
                values: new object[] { 0.5, 50.0 }
            );

            migrationBuilder.UpdateData(
                table: "species_stage_configs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
                columns: new[] { "amount_per100fish", "max_stocking_density" },
                values: new object[] { 3.0, 30.0 }
            );

            migrationBuilder.UpdateData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000501"),
                columns: new[] { "max_value", "min_value" },
                values: new object[] { 30.0, 26.0 }
            );

            migrationBuilder.UpdateData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000502"),
                columns: new[] { "max_value", "min_value" },
                values: new object[] { 8.0, 6.5 }
            );

            migrationBuilder.UpdateData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000503"),
                columns: new[] { "max_value", "min_value" },
                values: new object[] { 29.0, 25.0 }
            );

            migrationBuilder.UpdateData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000504"),
                columns: new[] { "max_value", "min_value" },
                values: new object[] { 8.5, 6.5 }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "min_value",
                table: "species_thresholds",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision"
            );

            migrationBuilder.AlterColumn<float>(
                name: "max_value",
                table: "species_thresholds",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision"
            );

            migrationBuilder.AlterColumn<float>(
                name: "max_stocking_density",
                table: "species_stage_configs",
                type: "real",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<float>(
                name: "amount_per100fish",
                table: "species_stage_configs",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision"
            );

            migrationBuilder.AlterColumn<float>(
                name: "quantity",
                table: "mortality_logs",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer"
            );

            migrationBuilder.AlterColumn<float>(
                name: "min_value",
                table: "jobs",
                type: "real",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<float>(
                name: "max_value",
                table: "jobs",
                type: "real",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<float>(
                name: "radius",
                table: "fish_tanks",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision"
            );

            migrationBuilder.AlterColumn<float>(
                name: "height",
                table: "fish_tanks",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision"
            );

            migrationBuilder.AlterColumn<float>(
                name: "amount",
                table: "feeding_logs",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision"
            );

            migrationBuilder.AlterColumn<float>(
                name: "protein_percentage",
                table: "feed_types",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision"
            );

            migrationBuilder.AlterColumn<float>(
                name: "initial_quantity",
                table: "farming_batches",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer"
            );

            migrationBuilder.AlterColumn<float>(
                name: "current_quantity",
                table: "farming_batches",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer"
            );

            migrationBuilder.AlterColumn<float>(
                name: "value",
                table: "alerts",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision"
            );

            migrationBuilder.UpdateData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001801"),
                column: "value",
                value: 31.2f
            );

            migrationBuilder.UpdateData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001802"),
                column: "value",
                value: 7.2f
            );

            migrationBuilder.UpdateData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001803"),
                column: "value",
                value: 28.5f
            );

            migrationBuilder.UpdateData(
                table: "farming_batches",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                columns: new[] { "current_quantity", "initial_quantity" },
                values: new object[] { 950f, 1000f }
            );

            migrationBuilder.UpdateData(
                table: "farming_batches",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001502"),
                columns: new[] { "current_quantity", "initial_quantity" },
                values: new object[] { 0f, 800f }
            );

            migrationBuilder.UpdateData(
                table: "feed_types",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000201"),
                column: "protein_percentage",
                value: 45f
            );

            migrationBuilder.UpdateData(
                table: "feed_types",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000202"),
                column: "protein_percentage",
                value: 38f
            );

            migrationBuilder.UpdateData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001601"),
                column: "amount",
                value: 5.5f
            );

            migrationBuilder.UpdateData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001602"),
                column: "amount",
                value: 5.8f
            );

            migrationBuilder.UpdateData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001603"),
                column: "amount",
                value: 6f
            );

            migrationBuilder.UpdateData(
                table: "fish_tanks",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                columns: new[] { "height", "radius" },
                values: new object[] { 0f, 0f }
            );

            migrationBuilder.UpdateData(
                table: "jobs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001001"),
                columns: new[] { "max_value", "min_value" },
                values: new object[] { null, null }
            );

            migrationBuilder.UpdateData(
                table: "jobs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001002"),
                columns: new[] { "max_value", "min_value" },
                values: new object[] { 30f, null }
            );

            migrationBuilder.UpdateData(
                table: "jobs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001003"),
                columns: new[] { "max_value", "min_value" },
                values: new object[] { null, null }
            );

            migrationBuilder.UpdateData(
                table: "mortality_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001701"),
                column: "quantity",
                value: 30f
            );

            migrationBuilder.UpdateData(
                table: "mortality_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001702"),
                column: "quantity",
                value: 20f
            );

            migrationBuilder.UpdateData(
                table: "species_stage_configs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000601"),
                columns: new[] { "amount_per100fish", "max_stocking_density" },
                values: new object[] { 0.5f, 50f }
            );

            migrationBuilder.UpdateData(
                table: "species_stage_configs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
                columns: new[] { "amount_per100fish", "max_stocking_density" },
                values: new object[] { 3f, 30f }
            );

            migrationBuilder.UpdateData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000501"),
                columns: new[] { "max_value", "min_value" },
                values: new object[] { 30f, 26f }
            );

            migrationBuilder.UpdateData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000502"),
                columns: new[] { "max_value", "min_value" },
                values: new object[] { 8f, 6.5f }
            );

            migrationBuilder.UpdateData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000503"),
                columns: new[] { "max_value", "min_value" },
                values: new object[] { 29f, 25f }
            );

            migrationBuilder.UpdateData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000504"),
                columns: new[] { "max_value", "min_value" },
                values: new object[] { 8.5f, 6.5f }
            );
        }
    }
}
