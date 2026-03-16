using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestrictSoftDeleteToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sensors_master_board_id_is_deleted",
                table: "sensors");

            migrationBuilder.DropIndex(
                name: "ix_master_boards_fish_tank_id_is_deleted",
                table: "master_boards");

            migrationBuilder.DropIndex(
                name: "ix_fish_tanks_farm_id_is_deleted",
                table: "fish_tanks");

            migrationBuilder.DropIndex(
                name: "ix_control_devices_master_board_id_is_deleted",
                table: "control_devices");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "sensors");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "sensors");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "master_boards");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "master_boards");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "jobs");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "fish_tanks");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "fish_tanks");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "farms");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "farms");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "control_devices");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "control_devices");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "cameras");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "cameras");

            migrationBuilder.CreateIndex(
                name: "ix_sensors_master_board_id",
                table: "sensors",
                column: "master_board_id");

            migrationBuilder.CreateIndex(
                name: "ix_master_boards_fish_tank_id",
                table: "master_boards",
                column: "fish_tank_id");

            migrationBuilder.CreateIndex(
                name: "ix_fish_tanks_farm_id",
                table: "fish_tanks",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_control_devices_master_board_id",
                table: "control_devices",
                column: "master_board_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sensors_master_board_id",
                table: "sensors");

            migrationBuilder.DropIndex(
                name: "ix_master_boards_fish_tank_id",
                table: "master_boards");

            migrationBuilder.DropIndex(
                name: "ix_fish_tanks_farm_id",
                table: "fish_tanks");

            migrationBuilder.DropIndex(
                name: "ix_control_devices_master_board_id",
                table: "control_devices");

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "sensors",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "sensors",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "master_boards",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "master_boards",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "jobs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "jobs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "fish_tanks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "fish_tanks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "farms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "farms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "control_devices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "control_devices",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "cameras",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "cameras",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "cameras",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000701"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "cameras",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000702"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "cameras",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000703"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "control_devices",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000801"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "control_devices",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000802"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "control_devices",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000803"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "farms",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "fish_tanks",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "jobs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001001"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "jobs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001002"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "jobs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001003"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "master_boards",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "master_boards",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001202"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "sensors",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "sensors",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "sensors",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001303"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "sensors",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001304"),
                columns: new[] { "deleted_at", "is_deleted" },
                values: new object[] { null, false });

            migrationBuilder.CreateIndex(
                name: "ix_sensors_master_board_id_is_deleted",
                table: "sensors",
                columns: new[] { "master_board_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_master_boards_fish_tank_id_is_deleted",
                table: "master_boards",
                columns: new[] { "fish_tank_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_fish_tanks_farm_id_is_deleted",
                table: "fish_tanks",
                columns: new[] { "farm_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_control_devices_master_board_id_is_deleted",
                table: "control_devices",
                columns: new[] { "master_board_id", "is_deleted" });
        }
    }
}
