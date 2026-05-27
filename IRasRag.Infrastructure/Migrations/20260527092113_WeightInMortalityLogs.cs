using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WeightInMortalityLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "lost_weight_kg",
                table: "mortality_logs",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0
            );

            migrationBuilder.UpdateData(
                table: "mortality_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001701"),
                column: "lost_weight_kg",
                value: 4.5
            );

            migrationBuilder.UpdateData(
                table: "mortality_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001702"),
                column: "lost_weight_kg",
                value: 3.0
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "lost_weight_kg", table: "mortality_logs");
        }
    }
}
