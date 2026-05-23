using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBatchStages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "batch_stage",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farming_batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    species_stage_config_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    estimated_start_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    estimated_end_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    expected_duration_days = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_batch_stage", x => x.id);
                    table.ForeignKey(
                        name: "fk_batch_stage_farming_batches_farming_batch_id",
                        column: x => x.farming_batch_id,
                        principalTable: "farming_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_batch_stage_species_stage_configs_species_stage_config_id",
                        column: x => x.species_stage_config_id,
                        principalTable: "species_stage_configs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_batch_stage_farming_batch_id_sequence",
                table: "batch_stage",
                columns: new[] { "farming_batch_id", "sequence" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_batch_stage_species_stage_config_id",
                table: "batch_stage",
                column: "species_stage_config_id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "batch_stage");
        }
    }
}
