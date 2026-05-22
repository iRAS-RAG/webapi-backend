using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteStageConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_species_stage_config_feed_types_feed_types_feed_type_id",
                table: "species_stage_config_feed_types"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_species_stage_config_feed_types_species_stage_configs_speci",
                table: "species_stage_config_feed_types"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_species_stage_config_feed_types_feed_types_feed_type_id",
                table: "species_stage_config_feed_types",
                column: "feed_type_id",
                principalTable: "feed_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_species_stage_config_feed_types_species_stage_configs_speci",
                table: "species_stage_config_feed_types",
                column: "species_stage_config_id",
                principalTable: "species_stage_configs",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_species_stage_config_feed_types_feed_types_feed_type_id",
                table: "species_stage_config_feed_types"
            );

            migrationBuilder.DropForeignKey(
                name: "fk_species_stage_config_feed_types_species_stage_configs_speci",
                table: "species_stage_config_feed_types"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_species_stage_config_feed_types_feed_types_feed_type_id",
                table: "species_stage_config_feed_types",
                column: "feed_type_id",
                principalTable: "feed_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "fk_species_stage_config_feed_types_species_stage_configs_speci",
                table: "species_stage_config_feed_types",
                column: "species_stage_config_id",
                principalTable: "species_stage_configs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict
            );
        }
    }
}
