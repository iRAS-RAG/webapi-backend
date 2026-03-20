using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RecreateSpeciesStageConfigFeedTypeJoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_species_stage_configs_feed_types_feed_type_id",
                table: "species_stage_configs");

            migrationBuilder.DropIndex(
                name: "ix_species_stage_configs_feed_type_id",
                table: "species_stage_configs");

            migrationBuilder.DropColumn(
                name: "feed_type_id",
                table: "species_stage_configs");

            migrationBuilder.CreateTable(
                name: "species_stage_config_feed_types",
                columns: table => new
                {
                    species_stage_config_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feed_type_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_species_stage_config_feed_types", x => new { x.species_stage_config_id, x.feed_type_id });
                    table.ForeignKey(
                        name: "fk_species_stage_config_feed_types_feed_types_feed_type_id",
                        column: x => x.feed_type_id,
                        principalTable: "feed_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_species_stage_config_feed_types_species_stage_configs_speci",
                        column: x => x.species_stage_config_id,
                        principalTable: "species_stage_configs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "species_stage_config_feed_types",
                columns: new[] { "feed_type_id", "species_stage_config_id" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000201"), new Guid("aaaaaaaa-0000-0000-0000-000000000601") },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000202"), new Guid("aaaaaaaa-0000-0000-0000-000000000602") }
                });

            migrationBuilder.CreateIndex(
                name: "ix_species_stage_config_feed_types_feed_type_id",
                table: "species_stage_config_feed_types",
                column: "feed_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "species_stage_config_feed_types");

            migrationBuilder.AddColumn<Guid>(
                name: "feed_type_id",
                table: "species_stage_configs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "species_stage_configs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000601"),
                column: "feed_type_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000000201"));

            migrationBuilder.UpdateData(
                table: "species_stage_configs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
                column: "feed_type_id",
                value: new Guid("aaaaaaaa-0000-0000-0000-000000000202"));

            migrationBuilder.CreateIndex(
                name: "ix_species_stage_configs_feed_type_id",
                table: "species_stage_configs",
                column: "feed_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_species_stage_configs_feed_types_feed_type_id",
                table: "species_stage_configs",
                column: "feed_type_id",
                principalTable: "feed_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
