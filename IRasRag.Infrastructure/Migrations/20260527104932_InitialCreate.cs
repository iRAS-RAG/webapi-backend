using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    last_name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    email = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    action = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    entity_type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    entity_id = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    old_value = table.Column<string>(type: "text", nullable: true),
                    new_value = table.Column<string>(type: "text", nullable: true),
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
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.id);
                }
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
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_control_device_types", x => x.id);
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
                    protein_percentage = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
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
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feed_types", x => x.id);
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
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_types", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
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
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
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
                    code = table.Column<string>(
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
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_species", x => x.id);
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
                    height = table.Column<double>(type: "double precision", nullable: false),
                    radius = table.Column<double>(type: "double precision", nullable: false),
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
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    first_name = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    last_name = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    password_hash = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
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
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
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
                    species_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("pk_growth_stages", x => x.id);
                    table.ForeignKey(
                        name: "fk_growth_stages_species_species_id",
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
                name: "documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    uploaded_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_url = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: false
                    ),
                    uploaded_at = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    rag_status = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false,
                        defaultValue: "Pending"
                    ),
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
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false),
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
                    type = table.Column<string>(type: "text", nullable: false),
                    is_consumed = table.Column<bool>(type: "boolean", nullable: false),
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
                name: "species_stage_configs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    species_id = table.Column<Guid>(type: "uuid", nullable: false),
                    growth_stage_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_per100fish = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    frequency_per_day = table.Column<int>(type: "integer", nullable: false),
                    max_stocking_density = table.Column<double>(
                        type: "double precision",
                        nullable: true
                    ),
                    expected_duration_days = table.Column<int>(type: "integer", nullable: true),
                    expected_weight_kg_per_fish = table.Column<double>(
                        type: "double precision",
                        nullable: true
                    ),
                    survival_rate = table.Column<double>(type: "double precision", nullable: true),
                    sequence = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("pk_species_stage_configs", x => x.id);
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
                    min_value = table.Column<double>(type: "double precision", nullable: false),
                    max_value = table.Column<double>(type: "double precision", nullable: false),
                    advisory_threshold_id = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: true
                    ),
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
                    current_stage_config_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    paused_reason = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: true
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
                    initial_quantity = table.Column<int>(type: "integer", nullable: false),
                    current_quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_of_measure = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    estimated_harvest_count = table.Column<int>(type: "integer", nullable: true),
                    estimated_harvest_weight_kg = table.Column<double>(
                        type: "double precision",
                        nullable: true
                    ),
                    species_id = table.Column<Guid>(type: "uuid", nullable: true),
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
                        principalColumn: "id"
                    );
                    table.ForeignKey(
                        name: "fk_farming_batches_species_stage_configs_current_stage_config_",
                        column: x => x.current_stage_config_id,
                        principalTable: "species_stage_configs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "species_stage_config_feed_types",
                columns: table => new
                {
                    species_stage_config_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feed_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "pk_species_stage_config_feed_types",
                        x => new { x.species_stage_config_id, x.feed_type_id }
                    );
                    table.ForeignKey(
                        name: "fk_species_stage_config_feed_types_feed_types_feed_type_id",
                        column: x => x.feed_type_id,
                        principalTable: "feed_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_species_stage_config_feed_types_species_stage_configs_speci",
                        column: x => x.species_stage_config_id,
                        principalTable: "species_stage_configs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
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
                    min_value = table.Column<double>(type: "double precision", nullable: true),
                    max_value = table.Column<double>(type: "double precision", nullable: true),
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
                    period_start = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    average = table.Column<double>(type: "double precision", nullable: false),
                    min = table.Column<double>(type: "double precision", nullable: false),
                    max = table.Column<double>(type: "double precision", nullable: false),
                    sample_count = table.Column<int>(type: "integer", nullable: false),
                    has_warning = table.Column<bool>(type: "boolean", nullable: false),
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
                name: "alerts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sensor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    species_threshold_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farming_batch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fish_tank_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sensor_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trigger_value = table.Column<double>(type: "double precision", nullable: false),
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
                        name: "fk_alerts_sensor_types_sensor_type_id",
                        column: x => x.sensor_type_id,
                        principalTable: "sensor_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_alerts_sensors_sensor_id",
                        column: x => x.sensor_id,
                        principalTable: "sensors",
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
                    actual_start_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    actual_end_date = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
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

            migrationBuilder.CreateTable(
                name: "feeding_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farming_batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feed_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<double>(type: "double precision", nullable: false),
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
                    table.ForeignKey(
                        name: "fk_feeding_logs_feed_types_feed_type_id",
                        column: x => x.feed_type_id,
                        principalTable: "feed_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_feeding_logs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
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
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    lost_weight_kg = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
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
                    table.ForeignKey(
                        name: "fk_mortality_logs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
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
                    document_id = table.Column<Guid>(type: "uuid", nullable: true),
                    suggestion_text = table.Column<string>(type: "text", nullable: false),
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
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.InsertData(
                table: "control_device_types",
                columns: new[] { "id", "created_at", "description", "modified_at", "name" },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000701"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Thiết bị bơm nước tuần hoàn trong hệ thống RAS",
                        null,
                        "Máy bơm nước",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000702"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Thiết bị cung cấp oxy hòa tan cho nước nuôi",
                        null,
                        "Máy sục khí",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000703"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Thiết bị cấp thức ăn tự động theo lịch định sẵn",
                        null,
                        "Máy cho ăn tự động",
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "farms",
                columns: new[]
                {
                    "id",
                    "address",
                    "created_at",
                    "email",
                    "modified_at",
                    "name",
                    "phone_number",
                },
                values: new object[]
                {
                    new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                    "Đường 123, Tp.HCM",
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    "contact@aquabluefarm.vn",
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
                    "description",
                    "manufacturer",
                    "modified_at",
                    "name",
                    "protein_percentage",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000201"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Thức ăn có hàm lượng protein cao, phù hợp cho giai đoạn đầu phát triển của cá.",
                        "AquaFeed Solutions",
                        null,
                        "Giàu protein",
                        45.0,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000202"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Thức ăn tiêu chuẩn, phù hợp cho giai đoạn phát triển tiếp theo của cá.",
                        "AquaFeed Solutions",
                        null,
                        "Tiêu chuẩn",
                        38.0,
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "job_types",
                columns: new[] { "id", "created_at", "description", "modified_at", "name" },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000901"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Tự động thực hiện theo thời gian đã định",
                        null,
                        "Công việc theo lịch",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000902"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Tự động thực hiện dựa trên giá trị cảm biến",
                        null,
                        "Công việc dựa trên cảm biến",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000903"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Thực hiện bằng tay khi cần thiết",
                        null,
                        "Công việc thủ công",
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "created_at", "modified_at", "name" },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "Admin",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "Supervisor",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "Operator",
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "sensor_types",
                columns: new[]
                {
                    "id",
                    "code",
                    "created_at",
                    "measure_type",
                    "modified_at",
                    "name",
                    "unit_of_measure",
                },
                values: new object[,]
                {
                    {
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        "waterTemp",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Nhiệt độ",
                        null,
                        "Nhiệt độ nước",
                        "Độ C",
                    },
                    {
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        "pH",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Tính axit",
                        null,
                        "Độ pH",
                        "pH",
                    },
                    {
                        new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                        "tds",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Tổng chất rắn hòa tan",
                        null,
                        "TDS",
                        "ppm",
                    },
                    {
                        new Guid("eeeeeeee-0000-0000-0000-000000000004"),
                        "flowRate",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Lưu lượng",
                        null,
                        "Lưu lượng nước",
                        "L/min",
                    },
                    {
                        new Guid("eeeeeeee-0000-0000-0000-000000000005"),
                        "waterLevel",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Mức nước",
                        null,
                        "Mực nước",
                        "0/1",
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "species",
                columns: new[] { "id", "created_at", "modified_at", "name" },
                values: new object[]
                {
                    new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    null,
                    "Cá rô phi",
                }
            );

            migrationBuilder.InsertData(
                table: "cameras",
                columns: new[] { "id", "created_at", "farm_id", "modified_at", "name", "url" },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000701"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                        null,
                        "Camera cổng chính",
                        "rtsp://192.168.1.100:554/stream1",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000702"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                        null,
                        "Camera giám sát bể số 1",
                        "rtsp://192.168.1.101:554/stream1",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000703"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                        null,
                        "Camera khu vực cho ăn",
                        "rtsp://192.168.1.102:554/stream1",
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
                    "farm_id",
                    "height",
                    "modified_at",
                    "name",
                    "radius",
                    "topic_code",
                },
                values: new object[]
                {
                    new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                    "rtsp://192.168.1.101:554/stream1",
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                    1.2,
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    "Bể nuôi số 1",
                    1.8,
                    "tank/001",
                }
            );

            migrationBuilder.InsertData(
                table: "growth_stages",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "description",
                    "modified_at",
                    "name",
                    "species_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Giai đoạn từ khi nở đến khi cá phát triển đủ lớn để chuyển sang giai đoạn cá giống.",
                        null,
                        "Cá bột",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Giai đoạn từ cá bột đến khi cá đạt kích thước thương phẩm.",
                        null,
                        "Cá giống",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "jobs",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "default_state",
                    "description",
                    "end_time",
                    "execution_days",
                    "is_active",
                    "job_type_id",
                    "max_value",
                    "min_value",
                    "modified_at",
                    "name",
                    "repeat_interval_minutes",
                    "sensor_id",
                    "start_time",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001001"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        "Cho cá ăn tự động vào lúc 6 giờ sáng mỗi ngày",
                        new TimeSpan(0, 6, 5, 0, 0),
                        "ALL",
                        true,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000901"),
                        null,
                        null,
                        null,
                        "Cho ăn buổi sáng",
                        null,
                        null,
                        new TimeSpan(0, 6, 0, 0, 0),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001003"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        "Sục khí 15 phút mỗi 3 giờ trong giờ làm việc",
                        new TimeSpan(0, 18, 0, 0, 0),
                        "ALL",
                        true,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000901"),
                        null,
                        null,
                        null,
                        "Sục khí định kỳ",
                        180,
                        null,
                        new TimeSpan(0, 6, 0, 0, 0),
                    },
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
                    "last_name",
                    "modified_at",
                    "password_hash",
                    "role_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "admin@example.com",
                        "Văn A",
                        false,
                        "Nguyễn",
                        null,
                        "$2a$11$TjsTmXlpjjTVPajZiLxCV.XPuTPgCgphg7sfC9Fs/YwSA4M4IYqYu",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "supervisor@example.com",
                        "Thị B",
                        false,
                        "Trần",
                        null,
                        "$2a$11$TjsTmXlpjjTVPajZiLxCV.XPuTPgCgphg7sfC9Fs/YwSA4M4IYqYu",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "operator@example.com",
                        "Văn C",
                        false,
                        "Lê",
                        null,
                        "$2a$11$TjsTmXlpjjTVPajZiLxCV.XPuTPgCgphg7sfC9Fs/YwSA4M4IYqYu",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "documents",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "file_url",
                    "modified_at",
                    "title",
                    "uploaded_at",
                    "uploaded_by_user_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001901"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "https://res.cloudinary.com/dhelnz7sw/raw/upload/v1779455970/IRAS-RAG/documents/KT_nu%C3%B4i_cua_bi%E1%BB%83n_lcsego.pdf",
                        null,
                        "Kĩ thuật nuôi cua biển",
                        new DateTime(2023, 12, 15, 10, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001902"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "https://res.cloudinary.com/dhelnz7sw/raw/upload/v1779455830/IRAS-RAG/documents/C%E1%BA%A9m_nang_nu%C3%B4i_c%C3%A1_n%C6%B0%E1%BB%9Bc_t%C4%A9nh_M%C3%A8_Tr%C3%B4i_Tr%E1%BA%AFm_Ch%C3%A9p_lfmxrd.pdf",
                        null,
                        "Cẩm nang nuôi cá nước tĩnh Mè Trôi Trắm Chép",
                        new DateTime(2023, 12, 20, 14, 30, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001903"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "https://res.cloudinary.com/dhelnz7sw/raw/upload/v1778650465/IRAS-RAG/documents/KT_nu%C3%B4i_c%C3%A1_r%C3%B4_siwemh.pdf",
                        null,
                        "Kĩ thuật nuôi cá rô",
                        new DateTime(2024, 1, 10, 9, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "master_boards",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "fish_tank_id",
                    "mac_address",
                    "modified_at",
                    "name",
                },
                values: new object[]
                {
                    new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                    "AA:BB:CC:DD:EE:01",
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    "Board điều khiển chính 1",
                }
            );

            migrationBuilder.InsertData(
                table: "species_stage_configs",
                columns: new[]
                {
                    "id",
                    "amount_per100fish",
                    "created_at",
                    "expected_duration_days",
                    "expected_weight_kg_per_fish",
                    "frequency_per_day",
                    "growth_stage_id",
                    "max_stocking_density",
                    "modified_at",
                    "sequence",
                    "species_id",
                    "survival_rate",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000601"),
                        0.5,
                        null,
                        30,
                        0.01,
                        6,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        50.0,
                        null,
                        1,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                        0.94999999999999996,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
                        3.0,
                        null,
                        90,
                        0.10000000000000001,
                        3,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        30.0,
                        null,
                        2,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                        0.97999999999999998,
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "species_thresholds",
                columns: new[]
                {
                    "id",
                    "advisory_threshold_id",
                    "created_at",
                    "growth_stage_id",
                    "max_value",
                    "min_value",
                    "modified_at",
                    "sensor_type_id",
                    "species_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000501"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        30.0,
                        26.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000502"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        8.0,
                        6.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000503"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        29.0,
                        25.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000504"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        8.5,
                        6.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "user_farms",
                columns: new[] { "id", "created_at", "farm_id", "modified_at", "user_id" },
                values: new object[,]
                {
                    {
                        new Guid("44444444-0001-0001-0001-000000000001"),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                    },
                    {
                        new Guid("44444444-0001-0001-0001-000000000002"),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "control_devices",
                columns: new[]
                {
                    "id",
                    "command_off",
                    "command_on",
                    "control_device_type_id",
                    "created_at",
                    "master_board_id",
                    "modified_at",
                    "name",
                    "pin_code",
                    "state",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000801"),
                        "PUMP1_OFF",
                        "PUMP1_ON",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000701"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Máy bơm chính 1",
                        5,
                        false,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000802"),
                        "AERATOR1_OFF",
                        "AERATOR1_ON",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000702"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Máy sục khí 1",
                        6,
                        false,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000803"),
                        "FEEDER1_OFF",
                        "FEEDER1_ON",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000703"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Máy cho ăn tự động 1",
                        7,
                        false,
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "farming_batches",
                columns: new[]
                {
                    "id",
                    "actual_harvest_date",
                    "created_at",
                    "current_quantity",
                    "current_stage_config_id",
                    "estimated_harvest_count",
                    "estimated_harvest_date",
                    "estimated_harvest_weight_kg",
                    "fish_tank_id",
                    "initial_quantity",
                    "modified_at",
                    "name",
                    "paused_reason",
                    "species_id",
                    "start_date",
                    "status",
                    "unit_of_measure",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        null,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        950,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000601"),
                        900,
                        new DateTime(2024, 7, 15, 0, 0, 0, 0, DateTimeKind.Utc),
                        90.0,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                        1000,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Lô nuôi cá rô phi 2024-01",
                        null,
                        null,
                        new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc),
                        "ACTIVE",
                        "con",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001502"),
                        new DateTime(2024, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        0,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
                        780,
                        new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        78.0,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                        800,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Lô nuôi cá rô phi 2023-12",
                        null,
                        null,
                        new DateTime(2023, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "HARVESTED",
                        "con",
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "sensors",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "master_board_id",
                    "modified_at",
                    "name",
                    "pin_code",
                    "sensor_type_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
                        "Cảm biến nhiệt độ 1",
                        2,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
                        "Cảm biến pH 1",
                        3,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001304"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
                        "Cảm biến oxy hòa tan 1",
                        4,
                        new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "species_stage_config_feed_types",
                columns: new[] { "feed_type_id", "species_stage_config_id" },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000201"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000601"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000202"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "alerts",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "farming_batch_id",
                    "fish_tank_id",
                    "modified_at",
                    "raised_at",
                    "resolved_at",
                    "sensor_id",
                    "sensor_type_id",
                    "species_threshold_id",
                    "status",
                    "trigger_value",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001801"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                        null,
                        new DateTime(2024, 1, 15, 14, 30, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 15, 18, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000501"),
                        "RESOLVED",
                        31.199999999999999,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001802"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                        null,
                        new DateTime(2024, 1, 16, 8, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 16, 10, 30, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000502"),
                        "RESOLVED",
                        7.2000000000000002,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001803"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                        null,
                        new DateTime(2024, 1, 17, 12, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000501"),
                        "ACKNOWLEDGED",
                        28.5,
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "feeding_logs",
                columns: new[]
                {
                    "id",
                    "amount",
                    "created_at",
                    "created_date",
                    "farming_batch_id",
                    "feed_type_id",
                    "modified_at",
                    "user_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001601"),
                        5.5,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 20, 6, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000201"),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001602"),
                        5.7999999999999998,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 20, 12, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000201"),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001603"),
                        6.0,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 20, 18, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000202"),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "job_control_mappings",
                columns: new[]
                {
                    "id",
                    "control_device_id",
                    "created_at",
                    "job_id",
                    "modified_at",
                    "target_state",
                    "trigger_condition",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001101"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000803"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001001"),
                        null,
                        true,
                        "ALWAYS",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001103"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000802"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001003"),
                        null,
                        true,
                        "ALWAYS",
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "jobs",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "default_state",
                    "description",
                    "end_time",
                    "execution_days",
                    "is_active",
                    "job_type_id",
                    "max_value",
                    "min_value",
                    "modified_at",
                    "name",
                    "repeat_interval_minutes",
                    "sensor_id",
                    "start_time",
                },
                values: new object[]
                {
                    new Guid("aaaaaaaa-0000-0000-0000-000000001002"),
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    false,
                    "Bật máy bơm khi nhiệt độ vượt quá 30°C",
                    null,
                    "ALL",
                    true,
                    new Guid("aaaaaaaa-0000-0000-0000-000000000902"),
                    30.0,
                    null,
                    null,
                    "Kiểm soát nhiệt độ",
                    null,
                    new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                    null,
                }
            );

            migrationBuilder.InsertData(
                table: "mortality_logs",
                columns: new[]
                {
                    "id",
                    "batch_id",
                    "created_at",
                    "date",
                    "lost_weight_kg",
                    "modified_at",
                    "quantity",
                    "user_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001701"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc),
                        4.5,
                        null,
                        30,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001702"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc),
                        3.0,
                        null,
                        20,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "sensor_logs",
                columns: new[]
                {
                    "id",
                    "average",
                    "created_at",
                    "has_warning",
                    "max",
                    "min",
                    "modified_at",
                    "period_start",
                    "sample_count",
                    "sensor_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001401"),
                        27.899999999999999,
                        new DateTime(2026, 1, 1, 4, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        28.399999999999999,
                        27.5,
                        null,
                        new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        8,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001402"),
                        28.600000000000001,
                        new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        29.100000000000001,
                        28.100000000000001,
                        null,
                        new DateTime(2026, 1, 1, 4, 0, 0, 0, DateTimeKind.Utc),
                        8,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001403"),
                        7.0999999999999996,
                        new DateTime(2026, 1, 1, 4, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        7.2000000000000002,
                        7.0,
                        null,
                        new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        8,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001404"),
                        29.800000000000001,
                        new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        30.399999999999999,
                        29.100000000000001,
                        null,
                        new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc),
                        8,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001405"),
                        31.100000000000001,
                        new DateTime(2026, 1, 1, 16, 0, 0, 0, DateTimeKind.Utc),
                        true,
                        31.800000000000001,
                        30.5,
                        null,
                        new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc),
                        8,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001406"),
                        30.300000000000001,
                        new DateTime(2026, 1, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        30.899999999999999,
                        29.800000000000001,
                        null,
                        new DateTime(2026, 1, 1, 16, 0, 0, 0, DateTimeKind.Utc),
                        8,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001407"),
                        29.0,
                        new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        29.5,
                        28.600000000000001,
                        null,
                        new DateTime(2026, 1, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                        8,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001409"),
                        7.2999999999999998,
                        new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        7.4000000000000004,
                        7.2000000000000002,
                        null,
                        new DateTime(2026, 1, 1, 4, 0, 0, 0, DateTimeKind.Utc),
                        8,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001410"),
                        7.5,
                        new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        7.5999999999999996,
                        7.4000000000000004,
                        null,
                        new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc),
                        8,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001411"),
                        7.4000000000000004,
                        new DateTime(2026, 1, 1, 16, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        7.5,
                        7.2999999999999998,
                        null,
                        new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc),
                        8,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001412"),
                        7.5999999999999996,
                        new DateTime(2026, 1, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        7.7000000000000002,
                        7.5,
                        null,
                        new DateTime(2026, 1, 1, 16, 0, 0, 0, DateTimeKind.Utc),
                        8,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001413"),
                        7.2000000000000002,
                        new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        7.2999999999999998,
                        7.0999999999999996,
                        null,
                        new DateTime(2026, 1, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                        8,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001414"),
                        6.7999999999999998,
                        new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        7.0999999999999996,
                        6.5,
                        null,
                        new DateTime(2026, 1, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                        8,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001304"),
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "corrective_actions",
                columns: new[]
                {
                    "id",
                    "action_taken",
                    "alert_id",
                    "created_at",
                    "modified_at",
                    "notes",
                    "timestamp",
                    "user_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002001"),
                        "Bật hệ thống làm mát và tăng lưu lượng nước tuần hoàn",
                        new Guid("aaaaaaaa-0000-0000-0000-000000001801"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "Nhiệt độ giảm từ 31.2°C xuống 29.5°C sau 2 giờ. Tiếp tục theo dõi.",
                        new DateTime(2024, 1, 15, 15, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002002"),
                        "Thêm vôi nông nghiệp để điều chỉnh pH lên mức tối ưu",
                        new Guid("aaaaaaaa-0000-0000-0000-000000001802"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "Đã thêm 500g vôi. pH tăng từ 7.2 lên 7.6 sau 1 giờ. Vấn đề đã được giải quyết.",
                        new DateTime(2024, 1, 16, 9, 15, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002003"),
                        "Kiểm tra hệ thống sục khí và làm sạch bộ lọc",
                        new Guid("aaaaaaaa-0000-0000-0000-000000001803"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "Đã vệ sinh bộ lọc cơ học. Hệ thống sục khí hoạt động bình thường. Nhiệt độ ổn định.",
                        new DateTime(2024, 1, 17, 13, 30, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "job_control_mappings",
                columns: new[]
                {
                    "id",
                    "control_device_id",
                    "created_at",
                    "job_id",
                    "modified_at",
                    "target_state",
                    "trigger_condition",
                },
                values: new object[]
                {
                    new Guid("aaaaaaaa-0000-0000-0000-000000001102"),
                    new Guid("aaaaaaaa-0000-0000-0000-000000000801"),
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    new Guid("aaaaaaaa-0000-0000-0000-000000001002"),
                    null,
                    true,
                    "ABOVE_MAX",
                }
            );

            migrationBuilder.InsertData(
                table: "recommendations",
                columns: new[]
                {
                    "id",
                    "alert_id",
                    "created_at",
                    "document_id",
                    "modified_at",
                    "suggestion_text",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002101"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001801"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001901"),
                        null,
                        "Áp dụng quy trình xử lý nhiệt độ cao trong tài liệu: Tăng lưu lượng nước tuần hoàn và bật hệ thống làm mát. Kiểm tra mức oxy hòa tan để đảm bảo cá không bị thiếu oxy.",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002102"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001802"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001902"),
                        null,
                        "Theo quy trình điều chỉnh pH: Thêm vôi nông nghiệp để tăng độ pH lên mức tối ưu (7.5-8.0). Theo dõi pH hàng ngày và điều chỉnh nếu cần.",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002103"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001803"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001903"),
                        null,
                        "Tham khảo hướng dẫn quản lý chất lượng nước: Duy trì nhiệt độ trong khoảng 25-30°C. Kiểm tra các thông số khác như oxy hòa tan, ammonia và nitrite để đảm bảo môi trường nuôi tối ưu.",
                    },
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_alerts_farming_batch_id_status",
                table: "alerts",
                columns: new[] { "farming_batch_id", "status" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_alerts_fish_tank_id_farming_batch_id_sensor_type_id",
                table: "alerts",
                columns: new[] { "fish_tank_id", "farming_batch_id", "sensor_type_id" },
                unique: true,
                filter: "\"status\" IN ('OPEN', 'ACKNOWLEDGED') AND \"farming_batch_id\" IS NOT NULL"
            );

            migrationBuilder.CreateIndex(
                name: "ix_alerts_fish_tank_id_sensor_type_id",
                table: "alerts",
                columns: new[] { "fish_tank_id", "sensor_type_id" },
                unique: true,
                filter: "\"status\" IN ('OPEN', 'ACKNOWLEDGED') AND \"farming_batch_id\" IS NULL"
            );

            migrationBuilder.CreateIndex(
                name: "ix_alerts_fish_tank_id_status",
                table: "alerts",
                columns: new[] { "fish_tank_id", "status" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_alerts_sensor_id_status",
                table: "alerts",
                columns: new[] { "sensor_id", "status" }
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
                name: "ix_audit_logs_action",
                table: "audit_logs",
                column: "action"
            );

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_entity_type",
                table: "audit_logs",
                column: "entity_type"
            );

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_timestamp",
                table: "audit_logs",
                column: "timestamp"
            );

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_user_id",
                table: "audit_logs",
                column: "user_id"
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
                name: "ix_control_devices_master_board_id",
                table: "control_devices",
                column: "master_board_id"
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
                name: "ix_farming_batches_current_stage_config_id",
                table: "farming_batches",
                column: "current_stage_config_id"
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
                name: "ix_feeding_logs_farming_batch_id_feed_type_id_created_date",
                table: "feeding_logs",
                columns: new[] { "farming_batch_id", "feed_type_id", "created_date" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_feeding_logs_feed_type_id",
                table: "feeding_logs",
                column: "feed_type_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_feeding_logs_user_id",
                table: "feeding_logs",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_fish_tanks_farm_id",
                table: "fish_tanks",
                column: "farm_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_growth_stages_species_id_name",
                table: "growth_stages",
                columns: new[] { "species_id", "name" },
                unique: true
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
                name: "ix_master_boards_fish_tank_id",
                table: "master_boards",
                column: "fish_tank_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_master_boards_mac_address",
                table: "master_boards",
                column: "mac_address",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_mortality_logs_batch_id_date",
                table: "mortality_logs",
                columns: new[] { "batch_id", "date" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_mortality_logs_user_id",
                table: "mortality_logs",
                column: "user_id"
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
                name: "ix_sensor_logs_sensor_id_period_start",
                table: "sensor_logs",
                columns: new[] { "sensor_id", "period_start" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_sensors_master_board_id",
                table: "sensors",
                column: "master_board_id"
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
                name: "ix_species_stage_config_feed_types_feed_type_id",
                table: "species_stage_config_feed_types",
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
                name: "ix_species_stage_configs_species_id_sequence",
                table: "species_stage_configs",
                columns: new[] { "species_id", "sequence" },
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
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                table: "users",
                column: "role_id"
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
            migrationBuilder.DropTable(name: "audit_logs");

            migrationBuilder.DropTable(name: "batch_stage");

            migrationBuilder.DropTable(name: "cameras");

            migrationBuilder.DropTable(name: "corrective_actions");

            migrationBuilder.DropTable(name: "feeding_logs");

            migrationBuilder.DropTable(name: "job_control_mappings");

            migrationBuilder.DropTable(name: "mortality_logs");

            migrationBuilder.DropTable(name: "recommendations");

            migrationBuilder.DropTable(name: "refresh_tokens");

            migrationBuilder.DropTable(name: "sensor_logs");

            migrationBuilder.DropTable(name: "species_stage_config_feed_types");

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

            migrationBuilder.DropTable(name: "sensors");

            migrationBuilder.DropTable(name: "species_thresholds");

            migrationBuilder.DropTable(name: "users");

            migrationBuilder.DropTable(name: "species_stage_configs");

            migrationBuilder.DropTable(name: "master_boards");

            migrationBuilder.DropTable(name: "sensor_types");

            migrationBuilder.DropTable(name: "roles");

            migrationBuilder.DropTable(name: "growth_stages");

            migrationBuilder.DropTable(name: "fish_tanks");

            migrationBuilder.DropTable(name: "species");

            migrationBuilder.DropTable(name: "farms");
        }
    }
}
