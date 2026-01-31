using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCoreEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_roles_name", table: "roles");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bbbbbbbb-0000-0000-0000-000000000001")
            );

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bbbbbbbb-0000-0000-0000-000000000002")
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP"
            );

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "roles",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "roles",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP"
            );

            migrationBuilder.CreateTable(
                name: "control_device_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    description = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_control_device_types", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    content = table.Column<string>(type: "text", nullable: false),
                    uploaded_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    uploaded_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_documents", x => x.id);
                    table.ForeignKey(
                        name: "fk_documents_users_uploaded_by_user_id",
                        column: x => x.uploaded_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "farms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    address = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    phone_number = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    email = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_farms", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "feed_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    description = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: false
                    ),
                    weight_per_unit = table.Column<float>(type: "real", nullable: false),
                    protein_percentage = table.Column<float>(type: "real", nullable: false),
                    manufacturer = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feed_types", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "growth_stages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    description = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_growth_stages", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "job_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    description = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_types", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    expire_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "sensor_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    measure_type = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    unit_of_measure = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sensor_types", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "species",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_species", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "verifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code_hash = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    expire_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    is_consumed = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_verifications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "cameras",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    url = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cameras", x => x.id);
                    table.ForeignKey(
                        name: "fk_cameras_farms_farm_id",
                        column: x => x.farm_id,
                        principalTable: "farms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "fish_tanks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    height = table.Column<float>(type: "real", nullable: false),
                    radius = table.Column<float>(type: "real", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    topic_code = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    camera_url = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fish_tanks", x => x.id);
                    table.ForeignKey(
                        name: "fk_fish_tanks_farms_farm_id",
                        column: x => x.farm_id,
                        principalTable: "farms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "user_farms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_farms", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_farms_farms_farm_id",
                        column: x => x.farm_id,
                        principalTable: "farms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_user_farms_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "species_stage_configs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    species_id = table.Column<Guid>(type: "uuid", nullable: false),
                    growth_stage_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feed_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_per100fish = table.Column<float>(type: "real", nullable: false),
                    frequency_per_day = table.Column<int>(type: "integer", nullable: false),
                    max_stocking_density = table.Column<float>(type: "real", nullable: true),
                    expected_duration_days = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_species_stage_configs", x => x.id);
                    table.ForeignKey(
                        name: "fk_species_stage_configs_feed_types_feed_type_id",
                        column: x => x.feed_type_id,
                        principalTable: "feed_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_species_stage_configs_growth_stages_growth_stage_id",
                        column: x => x.growth_stage_id,
                        principalTable: "growth_stages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_species_stage_configs_species_species_id",
                        column: x => x.species_id,
                        principalTable: "species",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "species_thresholds",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    species_id = table.Column<Guid>(type: "uuid", nullable: false),
                    growth_stage_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sensor_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    min_value = table.Column<float>(type: "real", nullable: false),
                    max_value = table.Column<float>(type: "real", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_species_thresholds", x => x.id);
                    table.ForeignKey(
                        name: "fk_species_thresholds_growth_stages_growth_stage_id",
                        column: x => x.growth_stage_id,
                        principalTable: "growth_stages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_species_thresholds_sensor_types_sensor_type_id",
                        column: x => x.sensor_type_id,
                        principalTable: "sensor_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_species_thresholds_species_species_id",
                        column: x => x.species_id,
                        principalTable: "species",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "farming_batches",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    fish_tank_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    species_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    start_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    estimated_harvest_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    actual_harvest_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    initial_quantity = table.Column<float>(type: "real", nullable: false),
                    current_quantity = table.Column<float>(type: "real", nullable: false),
                    unit_of_measure = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_farming_batches", x => x.id);
                    table.ForeignKey(
                        name: "fk_farming_batches_fish_tanks_fish_tank_id",
                        column: x => x.fish_tank_id,
                        principalTable: "fish_tanks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_farming_batches_species_species_id",
                        column: x => x.species_id,
                        principalTable: "species",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "master_boards",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    mac_address = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    fish_tank_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_master_boards", x => x.id);
                    table.ForeignKey(
                        name: "fk_master_boards_fish_tanks_fish_tank_id",
                        column: x => x.fish_tank_id,
                        principalTable: "fish_tanks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "feeding_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farming_batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<float>(type: "real", nullable: false),
                    created_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feeding_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_feeding_logs_farming_batches_farming_batch_id",
                        column: x => x.farming_batch_id,
                        principalTable: "farming_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "mortality_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<float>(type: "real", nullable: false),
                    date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mortality_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_mortality_logs_farming_batches_batch_id",
                        column: x => x.batch_id,
                        principalTable: "farming_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "control_devices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    pin_code = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<bool>(type: "boolean", nullable: false),
                    command_on = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    command_off = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    master_board_id = table.Column<Guid>(type: "uuid", nullable: false),
                    control_device_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_control_devices", x => x.id);
                    table.ForeignKey(
                        name: "fk_control_devices_control_device_types_control_device_type_id",
                        column: x => x.control_device_type_id,
                        principalTable: "control_device_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_control_devices_master_boards_master_board_id",
                        column: x => x.master_board_id,
                        principalTable: "master_boards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "sensors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    pin_code = table.Column<int>(type: "integer", nullable: false),
                    sensor_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    master_board_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sensors", x => x.id);
                    table.ForeignKey(
                        name: "fk_sensors_master_boards_master_board_id",
                        column: x => x.master_board_id,
                        principalTable: "master_boards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_sensors_sensor_types_sensor_type_id",
                        column: x => x.sensor_type_id,
                        principalTable: "sensor_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    description = table.Column<string>(type: "text", nullable: false),
                    job_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sensor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    min_value = table.Column<float>(type: "real", nullable: true),
                    max_value = table.Column<float>(type: "real", nullable: true),
                    default_state = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "time", nullable: true),
                    end_time = table.Column<TimeSpan>(type: "time", nullable: true),
                    repeat_interval_minutes = table.Column<int>(type: "integer", nullable: true),
                    execution_days = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jobs", x => x.id);
                    table.ForeignKey(
                        name: "fk_jobs_job_types_job_type_id",
                        column: x => x.job_type_id,
                        principalTable: "job_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_jobs_sensors_sensor_id",
                        column: x => x.sensor_id,
                        principalTable: "sensors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "sensor_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sensor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data = table.Column<double>(type: "double precision", nullable: false),
                    is_warning = table.Column<bool>(type: "boolean", nullable: false),
                    data_json = table.Column<string>(
                        type: "jsonb",
                        nullable: false,
                        defaultValue: "{}"
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sensor_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_sensor_logs_sensors_sensor_id",
                        column: x => x.sensor_id,
                        principalTable: "sensors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "job_control_mappings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    control_device_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_state = table.Column<bool>(type: "boolean", nullable: false),
                    trigger_condition = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_control_mappings", x => x.id);
                    table.ForeignKey(
                        name: "fk_job_control_mappings_control_devices_control_device_id",
                        column: x => x.control_device_id,
                        principalTable: "control_devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_job_control_mappings_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sensor_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    species_threshold_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farming_batch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fish_tank_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sensor_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<float>(type: "real", nullable: false),
                    raised_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    resolved_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    status = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_alerts", x => x.id);
                    table.ForeignKey(
                        name: "fk_alerts_farming_batches_farming_batch_id",
                        column: x => x.farming_batch_id,
                        principalTable: "farming_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "fk_alerts_fish_tanks_fish_tank_id",
                        column: x => x.fish_tank_id,
                        principalTable: "fish_tanks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_alerts_sensor_logs_sensor_log_id",
                        column: x => x.sensor_log_id,
                        principalTable: "sensor_logs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_alerts_sensor_types_sensor_type_id",
                        column: x => x.sensor_type_id,
                        principalTable: "sensor_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_alerts_species_thresholds_species_threshold_id",
                        column: x => x.species_threshold_id,
                        principalTable: "species_thresholds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "corrective_actions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    alert_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action_taken = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    notes = table.Column<string>(type: "text", nullable: false),
                    timestamp = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_corrective_actions", x => x.id);
                    table.ForeignKey(
                        name: "fk_corrective_actions_alerts_alert_id",
                        column: x => x.alert_id,
                        principalTable: "alerts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_corrective_actions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "recommendations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    alert_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    suggestion_text = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    modified_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recommendations", x => x.id);
                    table.ForeignKey(
                        name: "fk_recommendations_alerts_alert_id",
                        column: x => x.alert_id,
                        principalTable: "alerts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_recommendations_documents_document_id",
                        column: x => x.document_id,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.InsertData(
                table: "farms",
                columns: new[]
                {
                    "id",
                    "address",
                    "created_at",
                    "deleted_at",
                    "email",
                    "is_deleted",
                    "modified_at",
                    "name",
                    "phone_number",
                },
                values: new object[]
                {
                    new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                    "Đường 123, Tp.HCM",
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    null,
                    "contact@aquabluefarm.vn",
                    false,
                    null,
                    "Trang trại RAS mẫu",
                    "+84-123-456-789",
                }
            );

            migrationBuilder.InsertData(
                table: "feed_types",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "deleted_at",
                    "description",
                    "manufacturer",
                    "modified_at",
                    "name",
                    "protein_percentage",
                    "weight_per_unit",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000201"),
                        null,
                        null,
                        "Thức ăn có hàm lượng protein cao, phù hợp cho giai đoạn đầu phát triển của cá.",
                        "AquaFeed Solutions",
                        null,
                        "Giàu protein",
                        45f,
                        25f,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000202"),
                        null,
                        null,
                        "Thức ăn tiêu chuẩn, phù hợp cho giai đoạn phát triển tiếp theo của cá.",
                        "AquaFeed Solutions",
                        null,
                        "Tiêu chuẩn",
                        38f,
                        25f,
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "growth_stages",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "deleted_at",
                    "description",
                    "modified_at",
                    "name",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        null,
                        null,
                        "Giai đoạn từ khi nở đến khi cá phát triển đủ lớn để chuyển sang giai đoạn cá giống.",
                        null,
                        "Cá bột",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        null,
                        null,
                        "Giai đoạn từ cá bột đến khi cá đạt kích thước thương phẩm.",
                        null,
                        "Cá giống",
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "sensor_types",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "deleted_at",
                    "measure_type",
                    "modified_at",
                    "name",
                    "unit_of_measure",
                },
                values: new object[,]
                {
                    {
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        null,
                        null,
                        "Nhiệt độ",
                        null,
                        "Nhiệt độ nước",
                        "Độ C",
                    },
                    {
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        null,
                        null,
                        "Tính axit",
                        null,
                        "Độ pH",
                        "pH",
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "species",
                columns: new[] { "id", "created_at", "deleted_at", "modified_at", "name" },
                values: new object[]
                {
                    new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    null,
                    null,
                    null,
                    "Cá rô phi",
                }
            );

            migrationBuilder.InsertData(
                table: "users",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "deleted_at",
                    "email",
                    "first_name",
                    "is_deleted",
                    "is_verified",
                    "last_name",
                    "modified_at",
                    "password_hash",
                    "role_id",
                    "user_name",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "admin@example.com",
                        "",
                        false,
                        true,
                        "",
                        null,
                        "$2a$11$TjsTmXlpjjTVPajZiLxCV.XPuTPgCgphg7sfC9Fs/YwSA4M4IYqYu",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                        "admin",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "user@example.com",
                        "",
                        false,
                        true,
                        "",
                        null,
                        "$2a$11$TjsTmXlpjjTVPajZiLxCV.XPuTPgCgphg7sfC9Fs/YwSA4M4IYqYu",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                        "user",
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "fish_tanks",
                columns: new[]
                {
                    "id",
                    "camera_url",
                    "created_at",
                    "deleted_at",
                    "farm_id",
                    "height",
                    "is_deleted",
                    "modified_at",
                    "name",
                    "radius",
                    "topic_code",
                },
                values: new object[]
                {
                    new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                    "",
                    null,
                    null,
                    new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                    0f,
                    false,
                    null,
                    "Bể nuôi số 1",
                    0f,
                    "tank/001",
                }
            );

            migrationBuilder.InsertData(
                table: "species_stage_configs",
                columns: new[]
                {
                    "id",
                    "amount_per100fish",
                    "created_at",
                    "deleted_at",
                    "expected_duration_days",
                    "feed_type_id",
                    "frequency_per_day",
                    "growth_stage_id",
                    "max_stocking_density",
                    "modified_at",
                    "species_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000601"),
                        0.5f,
                        null,
                        null,
                        30,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000201"),
                        6,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        50f,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
                        3f,
                        null,
                        null,
                        90,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000202"),
                        3,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        30f,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "user_farms",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "deleted_at",
                    "farm_id",
                    "modified_at",
                    "user_id",
                },
                values: new object[]
                {
                    new Guid("44444444-0001-0001-0001-000000000001"),
                    null,
                    null,
                    new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                    null,
                    new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_alerts_farming_batch_id_status",
                table: "alerts",
                columns: new[] { "farming_batch_id", "status" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_alerts_fish_tank_id_status",
                table: "alerts",
                columns: new[] { "fish_tank_id", "status" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_alerts_sensor_log_id_status",
                table: "alerts",
                columns: new[] { "sensor_log_id", "status" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_alerts_sensor_type_id",
                table: "alerts",
                column: "sensor_type_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_alerts_species_threshold_id",
                table: "alerts",
                column: "species_threshold_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_cameras_farm_id",
                table: "cameras",
                column: "farm_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_control_devices_control_device_type_id",
                table: "control_devices",
                column: "control_device_type_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_control_devices_master_board_id_is_deleted",
                table: "control_devices",
                columns: new[] { "master_board_id", "is_deleted" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_control_devices_master_board_id_pin_code",
                table: "control_devices",
                columns: new[] { "master_board_id", "pin_code" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_corrective_actions_alert_id",
                table: "corrective_actions",
                column: "alert_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_corrective_actions_user_id",
                table: "corrective_actions",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_documents_uploaded_by_user_id",
                table: "documents",
                column: "uploaded_by_user_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_farming_batches_fish_tank_id",
                table: "farming_batches",
                column: "fish_tank_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_farming_batches_species_id",
                table: "farming_batches",
                column: "species_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_feeding_logs_farming_batch_id_created_date",
                table: "feeding_logs",
                columns: new[] { "farming_batch_id", "created_date" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_fish_tanks_farm_id_is_deleted",
                table: "fish_tanks",
                columns: new[] { "farm_id", "is_deleted" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_job_control_mappings_control_device_id",
                table: "job_control_mappings",
                column: "control_device_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_job_control_mappings_job_id_control_device_id",
                table: "job_control_mappings",
                columns: new[] { "job_id", "control_device_id" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_job_types_name",
                table: "job_types",
                column: "name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_jobs_job_type_id",
                table: "jobs",
                column: "job_type_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_jobs_sensor_id",
                table: "jobs",
                column: "sensor_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_master_boards_fish_tank_id_is_deleted",
                table: "master_boards",
                columns: new[] { "fish_tank_id", "is_deleted" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_master_boards_mac_address",
                table: "master_boards",
                column: "mac_address",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_mortality_logs_batch_id",
                table: "mortality_logs",
                column: "batch_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_recommendations_alert_id",
                table: "recommendations",
                column: "alert_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_recommendations_document_id",
                table: "recommendations",
                column: "document_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token_hash",
                table: "refresh_tokens",
                column: "token_hash"
            );

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_sensor_logs_sensor_id_created_at",
                table: "sensor_logs",
                columns: new[] { "sensor_id", "created_at" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_sensors_master_board_id_is_deleted",
                table: "sensors",
                columns: new[] { "master_board_id", "is_deleted" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_sensors_master_board_id_pin_code",
                table: "sensors",
                columns: new[] { "master_board_id", "pin_code" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_sensors_sensor_type_id",
                table: "sensors",
                column: "sensor_type_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_species_stage_configs_feed_type_id",
                table: "species_stage_configs",
                column: "feed_type_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_species_stage_configs_growth_stage_id",
                table: "species_stage_configs",
                column: "growth_stage_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_species_stage_configs_species_id_growth_stage_id",
                table: "species_stage_configs",
                columns: new[] { "species_id", "growth_stage_id" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_species_thresholds_growth_stage_id",
                table: "species_thresholds",
                column: "growth_stage_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_species_thresholds_sensor_type_id",
                table: "species_thresholds",
                column: "sensor_type_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_species_thresholds_species_id_growth_stage_id_sensor_type_id",
                table: "species_thresholds",
                columns: new[] { "species_id", "growth_stage_id", "sensor_type_id" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_user_farms_farm_id",
                table: "user_farms",
                column: "farm_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_user_farms_user_id_farm_id",
                table: "user_farms",
                columns: new[] { "user_id", "farm_id" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_verifications_is_consumed_expire_date",
                table: "verifications",
                columns: new[] { "is_consumed", "expire_date" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_verifications_user_id",
                table: "verifications",
                column: "user_id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "cameras");

            migrationBuilder.DropTable(name: "corrective_actions");

            migrationBuilder.DropTable(name: "feeding_logs");

            migrationBuilder.DropTable(name: "job_control_mappings");

            migrationBuilder.DropTable(name: "mortality_logs");

            migrationBuilder.DropTable(name: "recommendations");

            migrationBuilder.DropTable(name: "refresh_tokens");

            migrationBuilder.DropTable(name: "species_stage_configs");

            migrationBuilder.DropTable(name: "user_farms");

            migrationBuilder.DropTable(name: "verifications");

            migrationBuilder.DropTable(name: "control_devices");

            migrationBuilder.DropTable(name: "jobs");

            migrationBuilder.DropTable(name: "alerts");

            migrationBuilder.DropTable(name: "documents");

            migrationBuilder.DropTable(name: "feed_types");

            migrationBuilder.DropTable(name: "control_device_types");

            migrationBuilder.DropTable(name: "job_types");

            migrationBuilder.DropTable(name: "farming_batches");

            migrationBuilder.DropTable(name: "sensor_logs");

            migrationBuilder.DropTable(name: "species_thresholds");

            migrationBuilder.DropTable(name: "sensors");

            migrationBuilder.DropTable(name: "growth_stages");

            migrationBuilder.DropTable(name: "species");

            migrationBuilder.DropTable(name: "master_boards");

            migrationBuilder.DropTable(name: "sensor_types");

            migrationBuilder.DropTable(name: "fish_tanks");

            migrationBuilder.DropTable(name: "farms");

            migrationBuilder.DropIndex(name: "ix_users_email", table: "users");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000001")
            );

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000002")
            );

            migrationBuilder.DropColumn(name: "email", table: "users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "roles",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "roles",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true
            );

            migrationBuilder.InsertData(
                table: "users",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "deleted_at",
                    "first_name",
                    "is_deleted",
                    "is_verified",
                    "last_name",
                    "password_hash",
                    "role_id",
                    "user_name",
                },
                values: new object[,]
                {
                    {
                        new Guid("bbbbbbbb-0000-0000-0000-000000000001"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "",
                        false,
                        true,
                        "",
                        "$2a$11$TjsTmXlpjjTVPajZiLxCV.XPuTPgCgphg7sfC9Fs/YwSA4M4IYqYu",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                        "admin",
                    },
                    {
                        new Guid("bbbbbbbb-0000-0000-0000-000000000002"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "",
                        false,
                        true,
                        "",
                        "$2a$11$TjsTmXlpjjTVPajZiLxCV.XPuTPgCgphg7sfC9Fs/YwSA4M4IYqYu",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                        "user",
                    },
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_roles_name",
                table: "roles",
                column: "name",
                unique: true
            );
        }
    }
}
