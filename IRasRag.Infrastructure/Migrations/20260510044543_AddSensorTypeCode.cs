using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSensorTypeCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "sensor_types",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                column: "code",
                value: "waterTemp");

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                column: "code",
                value: "pH");

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                column: "code",
                value: "tds");

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: new Guid("eeeeeeee-0000-0000-0000-000000000004"),
                column: "code",
                value: "flowRate");

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: new Guid("eeeeeeee-0000-0000-0000-000000000005"),
                column: "code",
                value: "waterLevel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "code",
                table: "sensor_types");
        }
    }
}
