using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvisoryThresholdIdToSpeciesThreshold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "advisory_threshold_id",
                table: "species_thresholds",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "documents",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000501"),
                column: "advisory_threshold_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000502"),
                column: "advisory_threshold_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000503"),
                column: "advisory_threshold_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "species_thresholds",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000504"),
                column: "advisory_threshold_id",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "advisory_threshold_id",
                table: "species_thresholds");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "documents",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
