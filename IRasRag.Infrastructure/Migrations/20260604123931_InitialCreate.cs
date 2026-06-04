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
                    min_possible_value = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    max_possible_value = table.Column<double>(
                        type: "double precision",
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
                    actual_harvest_weight_kg = table.Column<double>(
                        type: "double precision",
                        nullable: true
                    ),
                    fcr = table.Column<double>(type: "double precision", nullable: true),
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
                    "Trang trại RAS HCM",
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
                        "Thức ăn dạng bột mịn, hàm lượng protein cao (≥40%), phù hợp cho giai đoạn cá bột.",
                        "Grobest Vietnam",
                        null,
                        "Giàu protein",
                        45.0,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000202"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Thức ăn viên nhỏ, hàm lượng protein trung bình (35–40%), phù hợp cho giai đoạn cá hương và cá giống.",
                        "Cargill Vietnam",
                        null,
                        "Tiêu chuẩn",
                        38.0,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000203"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Thức ăn viên lớn, hàm lượng protein vừa phải (28–32%), phù hợp cho giai đoạn cá thương phẩm đến khi thu hoạch.",
                        "Uni-President Vietnam",
                        null,
                        "Thương phẩm",
                        30.0,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000204"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Thức ăn dạng bột/cám mịn cho cua con, hàm lượng protein rất cao (≥48%), bổ sung khoáng chất cho quá trình lột xác của cua bột và cua giống.",
                        "CP Vietnam",
                        null,
                        "Cua giàu đạm",
                        48.0,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000205"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Thức ăn viên vừa cho cua thịt, hàm lượng protein cao (38–42%), phù hợp cho giai đoạn nuôi thương phẩm đến khi thu hoạch.",
                        "Tomboy Aquafeed",
                        null,
                        "Cua thương phẩm",
                        40.0,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000206"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Thức ăn dạng bột siêu mịn, hàm lượng protein rất cao (≥50%), giàu DHA và astaxanthin, mô phỏng dinh dưỡng từ Artemia/mysid. Dành cho giai đoạn mực ấu trùng và mực non mới chuyển đổi thức ăn.",
                        "Skretting Vietnam",
                        null,
                        "Mực ấu trùng",
                        50.0,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000207"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Thức ăn viên vừa, hàm lượng protein cao (42–46%), phù hợp cho giai đoạn mực non đến mực thương phẩm.",
                        "Cargill Vietnam",
                        null,
                        "Mực thương phẩm",
                        44.0,
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
                    "max_possible_value",
                    "measure_type",
                    "min_possible_value",
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
                        50.0,
                        "Nhiệt độ nước",
                        0.0,
                        null,
                        "Nhiệt độ",
                        "°C",
                    },
                    {
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        "pH",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        14.0,
                        "Tính axit",
                        0.0,
                        null,
                        "pH",
                        "pH",
                    },
                    {
                        new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                        "tds",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        10000.0,
                        "Tổng chất rắn hòa tan",
                        0.0,
                        null,
                        "TDS",
                        "ppm",
                    },
                    {
                        new Guid("eeeeeeee-0000-0000-0000-000000000004"),
                        "flowRate",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        500.0,
                        "Lưu lượng",
                        0.0,
                        null,
                        "Lưu lượng nước",
                        "L/min",
                    },
                    {
                        new Guid("eeeeeeee-0000-0000-0000-000000000005"),
                        "waterLevel",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        1.0,
                        "Mức nước",
                        0.0,
                        null,
                        "Mực nước",
                        "0/1",
                    },
                    {
                        new Guid("eeeeeeee-0000-0000-0000-000000000006"),
                        "voltage",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        500.0,
                        "Điện áp",
                        0.0,
                        null,
                        "Điện áp",
                        "V",
                    },
                    {
                        new Guid("eeeeeeee-0000-0000-0000-000000000007"),
                        "current",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        50.0,
                        "Dòng điện",
                        0.0,
                        null,
                        "Dòng điện",
                        "A",
                    },
                    {
                        new Guid("eeeeeeee-0000-0000-0000-000000000008"),
                        "power",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        50000.0,
                        "Công suất",
                        0.0,
                        null,
                        "Công suất PZEM",
                        "W",
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "species",
                columns: new[] { "id", "created_at", "modified_at", "name" },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "Cá rô phi",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "Cua biển",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "Mực lá",
                    },
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
                        "Giai đoạn từ khi nở đến 21 ngày tuổi. Cá có kích thước nhỏ (0.5–2 g), cần thức ăn giàu đạm (≥40%) dạng bột mịn, cho ăn 6–8 lần/ngày.",
                        null,
                        "Cá bột",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Giai đoạn từ 51 đến 110 ngày tuổi. Cá đạt 20–150 g, thức ăn viên vừa (30–35% đạm), cho ăn 3–4 lần/ngày.",
                        null,
                        "Cá giống",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000403"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Giai đoạn từ 22 đến 50 ngày tuổi. Cá đạt 2–20 g, chuyển sang thức ăn viên nhỏ (35–40% đạm), cho ăn 4–6 lần/ngày.",
                        null,
                        "Cá hương",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000404"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Giai đoạn từ 111 ngày tuổi đến khi thu hoạch (~200 ngày). Cá đạt 150–500+ g, thức ăn viên lớn (28–32% đạm), cho ăn 2–3 lần/ngày.",
                        null,
                        "Cá thương phẩm",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000405"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Giai đoạn từ khi ấu trùng megalopa bám vào giá thể đến 20 ngày tuổi. Cua đạt 0.02–0.5 g, cần thức ăn giàu đạm (≥48%) dạng bột mịn, cho ăn 5–6 lần/ngày. Tỷ lệ sống thấp do tập tính ăn thịt lẫn nhau.",
                        null,
                        "Cua bột",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000406"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Giai đoạn từ 21 đến 60 ngày tuổi. Cua đạt 0.5–50 g, thức ăn viên nhỏ (40–45% đạm), cho ăn 3–4 lần/ngày. Cần giá thể (lưới, ống nhựa) để giảm tỷ lệ ăn thịt đồng loại.",
                        null,
                        "Cua giống",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000407"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Giai đoạn từ 61 ngày tuổi đến khi thu hoạch (~150 ngày). Cua đạt 50–350+ g, thức ăn viên vừa (38–42% đạm) kết hợp cá tạp tươi, cho ăn 2–3 lần/ngày. Nuôi trong lồng riêng hoặc ao có giá thể để tránh đánh nhau.",
                        null,
                        "Cua thịt",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000408"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Giai đoạn từ khi nở đến 30 ngày tuổi. Mực đạt 0.001–0.5 g, sống trôi nổi (planktonic), cần thức ăn sống (Artemia, mysid) 6–8 lần/ngày. Rất nhạy cảm với ánh sáng và chất lượng nước. Bể nuôi phải dạng tròn, không góc cạnh để tránh mực va đập.",
                        null,
                        "Mực ấu trùng",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000409"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Giai đoạn từ 31 đến 90 ngày tuổi. Mực đạt 0.5–50 g, bắt đầu bám đáy, chuyển dần từ thức ăn sống sang thức ăn chết (tôm/cá băm nhỏ), cho ăn 4–5 lần/ngày. Tỷ lệ sống phụ thuộc vào chất lượng thức ăn chuyển đổi.",
                        null,
                        "Mực non",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000410"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Giai đoạn từ 91 ngày tuổi đến khi thu hoạch (~180 ngày). Mực đạt 50–400 g, ăn cá/tôm cắt miếng, cho ăn 2–3 lần/ngày. Mực trưởng thành có tập tính săn mồi mạnh, cần cho ăn đầy đủ để tránh ăn thịt lẫn nhau.",
                        null,
                        "Mực thương phẩm",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
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
                        "admin@iras-rag.com",
                        "Minh Tuấn",
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
                        "supervisor@iras-rag.com",
                        "Thị Hương",
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
                        "operator@iras-rag.com",
                        "Văn Hùng",
                        false,
                        "Lê",
                        null,
                        "$2a$11$TjsTmXlpjjTVPajZiLxCV.XPuTPgCgphg7sfC9Fs/YwSA4M4IYqYu",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
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
                    "68:FE:71:16:A5:18",
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
                        0.29999999999999999,
                        null,
                        21,
                        0.002,
                        7,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        300.0,
                        null,
                        1,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                        0.92000000000000004,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
                        2.5,
                        null,
                        60,
                        0.14999999999999999,
                        3,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        40.0,
                        null,
                        3,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                        0.96999999999999997,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000603"),
                        1.0,
                        null,
                        30,
                        0.02,
                        5,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000403"),
                        120.0,
                        null,
                        2,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                        0.94999999999999996,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000604"),
                        4.0,
                        null,
                        90,
                        0.5,
                        2,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000404"),
                        20.0,
                        null,
                        4,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                        0.97999999999999998,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000605"),
                        0.5,
                        null,
                        20,
                        0.00050000000000000001,
                        5,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000405"),
                        200.0,
                        null,
                        1,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                        0.80000000000000004,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000606"),
                        2.0,
                        null,
                        40,
                        0.050000000000000003,
                        4,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000406"),
                        50.0,
                        null,
                        2,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                        0.84999999999999998,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000607"),
                        5.0,
                        null,
                        90,
                        0.34999999999999998,
                        2,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000407"),
                        10.0,
                        null,
                        3,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                        0.90000000000000002,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000608"),
                        0.20000000000000001,
                        null,
                        30,
                        0.00050000000000000001,
                        7,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000408"),
                        500.0,
                        null,
                        1,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                        0.5,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000609"),
                        1.5,
                        null,
                        60,
                        0.050000000000000003,
                        5,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000409"),
                        100.0,
                        null,
                        2,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                        0.69999999999999996,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000610"),
                        4.0,
                        null,
                        90,
                        0.40000000000000002,
                        3,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000410"),
                        30.0,
                        null,
                        3,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                        0.84999999999999998,
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
                        29.0,
                        27.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000502"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        8.1999999999999993,
                        6.7999999999999998,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000503"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        400.0,
                        250.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000504"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        45.0,
                        30.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000004"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000505"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        1.0,
                        1.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000005"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000506"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        12.5,
                        11.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000006"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000507"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        1.5,
                        0.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000007"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000508"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000401"),
                        40.0,
                        20.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000008"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000509"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000403"),
                        29.5,
                        26.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000510"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000403"),
                        8.5,
                        6.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000511"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000403"),
                        400.0,
                        200.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000512"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000403"),
                        50.0,
                        35.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000004"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000513"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000403"),
                        1.0,
                        1.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000005"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000514"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000403"),
                        12.5,
                        11.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000006"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000515"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000403"),
                        2.0,
                        0.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000007"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000516"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000403"),
                        45.0,
                        25.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000008"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000517"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        30.0,
                        26.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000518"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        8.5,
                        6.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000519"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        450.0,
                        200.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000520"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        55.0,
                        40.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000004"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000521"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        1.0,
                        1.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000005"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000522"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        12.5,
                        11.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000006"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000523"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        2.5,
                        0.80000000000000004,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000007"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000524"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000402"),
                        50.0,
                        30.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000008"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000525"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000404"),
                        30.0,
                        26.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000526"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000404"),
                        8.5,
                        6.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000527"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000404"),
                        450.0,
                        200.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000528"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000404"),
                        55.0,
                        40.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000004"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000529"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000404"),
                        1.0,
                        1.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000005"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000530"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000404"),
                        12.5,
                        11.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000006"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000531"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000404"),
                        3.0,
                        1.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000007"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000532"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000404"),
                        60.0,
                        30.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000008"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000533"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000405"),
                        30.5,
                        27.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000534"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000405"),
                        8.5,
                        7.7999999999999998,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000535"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000405"),
                        400.0,
                        250.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000536"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000405"),
                        40.0,
                        25.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000004"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000537"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000405"),
                        1.0,
                        1.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000005"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000538"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000405"),
                        12.5,
                        11.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000006"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000539"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000405"),
                        1.5,
                        0.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000007"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000540"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000405"),
                        35.0,
                        20.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000008"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000541"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000406"),
                        31.0,
                        27.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000542"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000406"),
                        8.5,
                        7.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000543"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000406"),
                        400.0,
                        200.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000544"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000406"),
                        50.0,
                        30.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000004"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000545"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000406"),
                        1.0,
                        1.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000005"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000546"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000406"),
                        12.5,
                        11.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000006"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000547"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000406"),
                        2.0,
                        0.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000007"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000548"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000406"),
                        45.0,
                        25.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000008"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000549"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000407"),
                        31.0,
                        26.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000550"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000407"),
                        8.5,
                        7.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000551"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000407"),
                        450.0,
                        200.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000552"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000407"),
                        55.0,
                        35.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000004"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000553"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000407"),
                        1.0,
                        1.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000005"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000554"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000407"),
                        12.5,
                        11.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000006"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000555"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000407"),
                        3.0,
                        1.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000007"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000556"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000407"),
                        55.0,
                        30.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000008"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000102"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000557"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000408"),
                        27.0,
                        25.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000558"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000408"),
                        8.3000000000000007,
                        8.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000559"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000408"),
                        500.0,
                        350.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000560"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000408"),
                        45.0,
                        30.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000004"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000561"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000408"),
                        1.0,
                        1.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000005"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000562"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000408"),
                        12.5,
                        11.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000006"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000563"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000408"),
                        1.5,
                        0.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000007"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000564"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000408"),
                        35.0,
                        20.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000008"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000565"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000409"),
                        28.0,
                        24.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000566"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000409"),
                        8.3000000000000007,
                        7.7999999999999998,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000567"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000409"),
                        500.0,
                        300.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000568"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000409"),
                        50.0,
                        35.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000004"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000569"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000409"),
                        1.0,
                        1.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000005"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000570"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000409"),
                        12.5,
                        11.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000006"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000571"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000409"),
                        2.0,
                        0.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000007"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000572"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000409"),
                        45.0,
                        25.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000008"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000573"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000410"),
                        28.0,
                        24.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000574"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000410"),
                        8.3000000000000007,
                        7.7999999999999998,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000575"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000410"),
                        500.0,
                        300.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000576"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000410"),
                        55.0,
                        40.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000004"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000577"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000410"),
                        1.0,
                        1.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000005"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000578"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000410"),
                        12.5,
                        11.5,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000006"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000579"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000410"),
                        3.0,
                        1.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000007"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000580"),
                        null,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000410"),
                        55.0,
                        30.0,
                        null,
                        new Guid("eeeeeeee-0000-0000-0000-000000000008"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000103"),
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
                values: new object[]
                {
                    new Guid("aaaaaaaa-0000-0000-0000-000000000801"),
                    "PUMP1_OFF",
                    "PUMP1_ON",
                    new Guid("aaaaaaaa-0000-0000-0000-000000000701"),
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    "Máy bơm nước",
                    9,
                    false,
                }
            );

            migrationBuilder.InsertData(
                table: "farming_batches",
                columns: new[]
                {
                    "id",
                    "actual_harvest_date",
                    "actual_harvest_weight_kg",
                    "created_at",
                    "current_quantity",
                    "current_stage_config_id",
                    "estimated_harvest_count",
                    "estimated_harvest_date",
                    "estimated_harvest_weight_kg",
                    "fcr",
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
                        new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc),
                        412.5,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        825,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000604"),
                        830,
                        new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Utc),
                        415.0,
                        1.55,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                        1000,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Vụ nuôi cá rô phi 2025-08",
                        null,
                        null,
                        new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "HARVESTED",
                        "con",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001502"),
                        new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc),
                        73.5,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        700,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
                        680,
                        new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Utc),
                        70.0,
                        1.3,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                        800,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "Vụ nuôi cá rô phi 2026-03",
                        null,
                        null,
                        new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc),
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
                        1,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
                        "Cảm biến pH 1",
                        2,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001303"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
                        "Cảm biến TDS 1",
                        3,
                        new Guid("eeeeeeee-0000-0000-0000-000000000003"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001304"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
                        "Cảm biến lưu lượng nước 1",
                        4,
                        new Guid("eeeeeeee-0000-0000-0000-000000000004"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001305"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
                        "Cảm biến mực nước 1",
                        5,
                        new Guid("eeeeeeee-0000-0000-0000-000000000005"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001306"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
                        "Cảm biến điện áp 1",
                        6,
                        new Guid("eeeeeeee-0000-0000-0000-000000000006"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001307"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
                        "Cảm biến dòng điện 1",
                        7,
                        new Guid("eeeeeeee-0000-0000-0000-000000000007"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001308"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
                        "Cảm biến công suất PZEM 1",
                        8,
                        new Guid("eeeeeeee-0000-0000-0000-000000000008"),
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
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000203"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000202"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000603"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000203"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000604"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000204"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000605"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000204"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000606"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000205"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000606"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000205"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000607"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000206"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000608"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000206"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000609"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000207"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000609"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000207"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000610"),
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
                        new DateTime(2025, 8, 10, 14, 30, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 8, 10, 18, 0, 0, 0, DateTimeKind.Utc),
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
                        new DateTime(2025, 8, 14, 8, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 8, 14, 10, 30, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000502"),
                        "RESOLVED",
                        8.4000000000000004,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001803"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                        null,
                        new DateTime(2025, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000509"),
                        "ACKNOWLEDGED",
                        30.199999999999999,
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "batch_stage",
                columns: new[]
                {
                    "id",
                    "actual_end_date",
                    "actual_start_date",
                    "created_at",
                    "estimated_end_date",
                    "estimated_start_date",
                    "expected_duration_days",
                    "farming_batch_id",
                    "modified_at",
                    "sequence",
                    "species_stage_config_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002101"),
                        new DateTime(2025, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        21,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        1,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000601"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002102"),
                        new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc),
                        30,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        2,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000603"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002103"),
                        new DateTime(2025, 11, 21, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 11, 20, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc),
                        60,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        3,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002104"),
                        new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 11, 22, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 11, 21, 0, 0, 0, 0, DateTimeKind.Utc),
                        90,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        4,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000604"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002201"),
                        new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        21,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001502"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        1,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000601"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002202"),
                        new DateTime(2026, 4, 21, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2026, 4, 21, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Utc),
                        30,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001502"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        2,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000603"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002203"),
                        new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2026, 4, 21, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2026, 4, 21, 0, 0, 0, 0, DateTimeKind.Utc),
                        60,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001502"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        3,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000602"),
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
                        3.0,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 8, 10, 6, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000201"),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001602"),
                        2.7999999999999998,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 8, 14, 12, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000201"),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001603"),
                        9.0,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 9, 5, 18, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000202"),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
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
                        new DateTime(2025, 9, 10, 0, 0, 0, 0, DateTimeKind.Utc),
                        0.45000000000000001,
                        null,
                        30,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000003"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001702"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc),
                        2.0,
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
                        44.799999999999997,
                        new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc),
                        false,
                        47.299999999999997,
                        42.100000000000001,
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
                    new Guid("aaaaaaaa-0000-0000-0000-000000001101"),
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
                        null,
                        null,
                        "Áp dụng quy trình xử lý nhiệt độ cao trong tài liệu: Tăng lưu lượng nước tuần hoàn và bật hệ thống làm mát. Kiểm tra mức oxy hòa tan để đảm bảo cá không bị thiếu oxy.",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002102"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001802"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        null,
                        "Theo quy trình điều chỉnh pH: Thêm vôi nông nghiệp để tăng độ pH lên mức tối ưu (7.5-8.0). Theo dõi pH hàng ngày và điều chỉnh nếu cần.",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000002103"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001803"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
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
