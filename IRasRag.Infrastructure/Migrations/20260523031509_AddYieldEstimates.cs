using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYieldEstimates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "expected_weight_kg_per_fish",
                table: "species_stage_configs",
                type: "double precision",
                nullable: true
            );

            migrationBuilder.AddColumn<double>(
                name: "survival_rate",
                table: "species_stage_configs",
                type: "double precision",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "estimated_harvest_count",
                table: "farming_batches",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<double>(
                name: "estimated_harvest_weight_kg",
                table: "farming_batches",
                type: "double precision",
                nullable: true
            );

            migrationBuilder.UpdateData(
                table: "farming_batches",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                columns: new[] { "estimated_harvest_count", "estimated_harvest_weight_kg" },
                values: new object[] { null, null }
            );

            migrationBuilder.UpdateData(
                table: "farming_batches",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001502"),
                columns: new[] { "estimated_harvest_count", "estimated_harvest_weight_kg" },
                values: new object[] { null, null }
            );

            migrationBuilder.UpdateData(
                table: "species_stage_configs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000601"),
                columns: new[] { "expected_weight_kg_per_fish", "survival_rate" },
                values: new object[] { null, null }
            );

            migrationBuilder.UpdateData(
                table: "species_stage_configs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
                columns: new[] { "expected_weight_kg_per_fish", "survival_rate" },
                values: new object[] { null, null }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "expected_weight_kg_per_fish",
                table: "species_stage_configs"
            );

            migrationBuilder.DropColumn(name: "survival_rate", table: "species_stage_configs");

            migrationBuilder.DropColumn(name: "estimated_harvest_count", table: "farming_batches");

            migrationBuilder.DropColumn(
                name: "estimated_harvest_weight_kg",
                table: "farming_batches"
            );
        }
    }
}
