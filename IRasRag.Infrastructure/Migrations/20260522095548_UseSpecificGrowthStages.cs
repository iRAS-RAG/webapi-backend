using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UseSpecificGrowthStages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_growth_stages_name", table: "growth_stages");

            migrationBuilder.AddColumn<int>(
                name: "sequence",
                table: "species_stage_configs",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<Guid>(
                name: "species_id",
                table: "growth_stages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.UpdateData(
                table: "growth_stages",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                column: "species_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000000101")
            );

            migrationBuilder.UpdateData(
                table: "growth_stages",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                column: "species_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000000101")
            );

            migrationBuilder.UpdateData(
                table: "species_stage_configs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000601"),
                column: "sequence",
                value: 1
            );

            migrationBuilder.UpdateData(
                table: "species_stage_configs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
                column: "sequence",
                value: 2
            );

            migrationBuilder.CreateIndex(
                name: "ix_species_stage_configs_species_id_sequence",
                table: "species_stage_configs",
                columns: new[] { "species_id", "sequence" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_growth_stages_species_id_name",
                table: "growth_stages",
                columns: new[] { "species_id", "name" },
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "fk_growth_stages_species_species_id",
                table: "growth_stages",
                column: "species_id",
                principalTable: "species",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_growth_stages_species_species_id",
                table: "growth_stages"
            );

            migrationBuilder.DropIndex(
                name: "ix_species_stage_configs_species_id_sequence",
                table: "species_stage_configs"
            );

            migrationBuilder.DropIndex(
                name: "ix_growth_stages_species_id_name",
                table: "growth_stages"
            );

            migrationBuilder.DropColumn(name: "sequence", table: "species_stage_configs");

            migrationBuilder.DropColumn(name: "species_id", table: "growth_stages");

            migrationBuilder.CreateIndex(
                name: "ix_growth_stages_name",
                table: "growth_stages",
                column: "name",
                unique: true
            );
        }
    }
}
