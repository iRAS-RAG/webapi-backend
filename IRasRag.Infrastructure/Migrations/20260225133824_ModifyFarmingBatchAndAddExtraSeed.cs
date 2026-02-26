using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyFarmingBatchAndAddExtraSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_farming_batches_species_species_id",
                table: "farming_batches");

            migrationBuilder.AlterColumn<Guid>(
                name: "species_id",
                table: "farming_batches",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "current_stage_config_id",
                table: "farming_batches",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "paused_reason",
                table: "farming_batches",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "farming_batches",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                columns: new[] { "current_stage_config_id", "paused_reason", "species_id" },
                values: new object[] { new Guid("aaaaaaaa-0000-0000-0000-000000000601"), null, null });

            migrationBuilder.UpdateData(
                table: "farming_batches",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001502"),
                columns: new[] { "current_stage_config_id", "paused_reason", "species_id" },
                values: new object[] { new Guid("aaaaaaaa-0000-0000-0000-000000000602"), null, null });

            migrationBuilder.InsertData(
                table: "sensor_types",
                columns: new[] { "id", "created_at", "measure_type", "modified_at", "name", "unit_of_measure" },
                values: new object[] { new Guid("eeeeeeee-0000-0000-0000-000000000003"), null, "Nồng độ oxy", null, "Oxy hòa tan", "mg/L" });

            migrationBuilder.InsertData(
                table: "user_farms",
                columns: new[] { "id", "created_at", "farm_id", "modified_at", "user_id" },
                values: new object[] { new Guid("44444444-0001-0001-0001-000000000002"), null, new Guid("aaaaaaaa-0000-0000-0000-000000000001"), null, new Guid("aaaaaaaa-0000-0000-0000-000000000003") });

            migrationBuilder.InsertData(
                table: "sensors",
                columns: new[] { "id", "created_at", "deleted_at", "is_deleted", "master_board_id", "modified_at", "name", "pin_code", "sensor_type_id" },
                values: new object[] { new Guid("aaaaaaaa-0000-0000-0000-000000001304"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("aaaaaaaa-0000-0000-0000-000000001201"), null, "Cảm biến oxy hòa tan 1", 4, new Guid("eeeeeeee-0000-0000-0000-000000000003") });

            migrationBuilder.InsertData(
                table: "sensor_logs",
                columns: new[] { "id", "created_at", "data", "data_json", "is_warning", "modified_at", "sensor_id" },
                values: new object[] { new Guid("aaaaaaaa-0000-0000-0000-000000001404"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6.7999999999999998, "{\"dissolvedOxygen\": 6.8, \"unit\": \"mg/L\"}", false, null, new Guid("aaaaaaaa-0000-0000-0000-000000001304") });

            migrationBuilder.CreateIndex(
                name: "ix_farming_batches_current_stage_config_id",
                table: "farming_batches",
                column: "current_stage_config_id");

            migrationBuilder.AddForeignKey(
                name: "fk_farming_batches_species_species_id",
                table: "farming_batches",
                column: "species_id",
                principalTable: "species",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_farming_batches_species_stage_configs_current_stage_config_",
                table: "farming_batches",
                column: "current_stage_config_id",
                principalTable: "species_stage_configs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_farming_batches_species_species_id",
                table: "farming_batches");

            migrationBuilder.DropForeignKey(
                name: "fk_farming_batches_species_stage_configs_current_stage_config_",
                table: "farming_batches");

            migrationBuilder.DropIndex(
                name: "ix_farming_batches_current_stage_config_id",
                table: "farming_batches");

            migrationBuilder.DeleteData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001404"));

            migrationBuilder.DeleteData(
                table: "user_farms",
                keyColumn: "id",
                keyValue: new Guid("44444444-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "sensors",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001304"));

            migrationBuilder.DeleteData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: new Guid("eeeeeeee-0000-0000-0000-000000000003"));

            migrationBuilder.DropColumn(
                name: "current_stage_config_id",
                table: "farming_batches");

            migrationBuilder.DropColumn(
                name: "paused_reason",
                table: "farming_batches");

            migrationBuilder.AlterColumn<Guid>(
                name: "species_id",
                table: "farming_batches",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "farming_batches",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                column: "species_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000000101"));

            migrationBuilder.UpdateData(
                table: "farming_batches",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001502"),
                column: "species_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000000101"));

            migrationBuilder.AddForeignKey(
                name: "fk_farming_batches_species_species_id",
                table: "farming_batches",
                column: "species_id",
                principalTable: "species",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
