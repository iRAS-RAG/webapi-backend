using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IRasRag.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "cameras",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "deleted_at",
                    "farm_id",
                    "is_deleted",
                    "modified_at",
                    "name",
                    "url",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000701"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                        false,
                        null,
                        "Camera cổng chính",
                        "rtsp://192.168.1.100:554/stream1",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000702"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                        false,
                        null,
                        "Camera giám sát bể số 1",
                        "rtsp://192.168.1.101:554/stream1",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000000703"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                        false,
                        null,
                        "Camera khu vực cho ăn",
                        "rtsp://192.168.1.102:554/stream1",
                    },
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
                table: "documents",
                columns: new[]
                {
                    "id",
                    "content",
                    "created_at",
                    "modified_at",
                    "title",
                    "uploaded_at",
                    "uploaded_by_user_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001901"),
                        "Khi nhiệt độ nước vượt quá 30°C:\n1. Tăng lưu lượng nước tuần hoàn\n2. Bật hệ thống làm mát\n3. Giảm mật độ thả nuôi nếu cần\n4. Kiểm tra hàm lượng oxy hòa tan\n5. Theo dõi hành vi của cá",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "Hướng dẫn xử lý nhiệt độ cao trong bể nuôi",
                        new DateTime(2023, 12, 15, 10, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001902"),
                        "Để duy trì độ pH ổn định:\n1. Kiểm tra độ kiềm của nước\n2. Sử dụng vôi nông nghiệp để tăng pH\n3. Sử dụng axit citric để giảm pH\n4. Theo dõi pH hàng ngày\n5. Đảm bảo hệ thống lọc sinh học hoạt động tốt",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "Quy trình điều chỉnh độ pH trong hệ thống RAS",
                        new DateTime(2023, 12, 20, 14, 30, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000002"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001903"),
                        "Các thông số quan trọng cần theo dõi:\n1. Nhiệt độ: 25-30°C\n2. pH: 6.5-8.5\n3. Oxy hòa tan: >5 mg/L\n4. Ammonia: <0.1 mg/L\n5. Nitrite: <0.2 mg/L\n6. Nitrate: <50 mg/L\nThực hiện kiểm tra hàng ngày và ghi chép đầy đủ.",
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        "Hướng dẫn quản lý chất lượng nước trong nuôi trồng thủy sản",
                        new DateTime(2024, 1, 10, 9, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
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
                    "estimated_harvest_date",
                    "fish_tank_id",
                    "initial_quantity",
                    "modified_at",
                    "name",
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
                        950f,
                        new DateTime(2024, 7, 15, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                        1000f,
                        null,
                        "Lô nuôi cá rô phi 2024-01",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                        new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc),
                        "ACTIVE",
                        "con",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001502"),
                        new DateTime(2024, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        0f,
                        new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                        800f,
                        null,
                        "Lô nuôi cá rô phi 2023-12",
                        new Guid("aaaaaaaa-0000-0000-0000-000000000101"),
                        new DateTime(2023, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        "HARVESTED",
                        "con",
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
                table: "master_boards",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "deleted_at",
                    "fish_tank_id",
                    "is_deleted",
                    "mac_address",
                    "modified_at",
                    "name",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                        false,
                        "AA:BB:CC:DD:EE:01",
                        null,
                        "Board điều khiển chính 1",
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001202"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                        false,
                        "AA:BB:CC:DD:EE:02",
                        null,
                        "Board điều khiển chính 2",
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
                    "deleted_at",
                    "is_deleted",
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
                        null,
                        false,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
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
                        null,
                        false,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
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
                        null,
                        false,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
                        "Máy cho ăn tự động 1",
                        7,
                        false,
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
                    "modified_at",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001601"),
                        5.5f,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 20, 6, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        null,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001602"),
                        5.8f,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 20, 12, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        null,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001603"),
                        6f,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 20, 18, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        null,
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
                    "deleted_at",
                    "description",
                    "end_time",
                    "execution_days",
                    "is_active",
                    "is_deleted",
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
                        null,
                        "Cho cá ăn tự động vào lúc 6 giờ sáng mỗi ngày",
                        new TimeSpan(0, 6, 5, 0, 0),
                        "ALL",
                        true,
                        false,
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
                        null,
                        "Sục khí 15 phút mỗi 3 giờ trong giờ làm việc",
                        new TimeSpan(0, 18, 0, 0, 0),
                        "ALL",
                        true,
                        false,
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
                table: "mortality_logs",
                columns: new[]
                {
                    "id",
                    "batch_id",
                    "created_at",
                    "date",
                    "modified_at",
                    "quantity",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001701"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        30f,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001702"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        20f,
                    },
                }
            );

            migrationBuilder.InsertData(
                table: "sensors",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "deleted_at",
                    "is_deleted",
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
                        null,
                        false,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
                        "Cảm biến nhiệt độ 1",
                        2,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        false,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001201"),
                        null,
                        "Cảm biến pH 1",
                        3,
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001303"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        false,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001202"),
                        null,
                        "Cảm biến nhiệt độ 2",
                        2,
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
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
                    "deleted_at",
                    "description",
                    "end_time",
                    "execution_days",
                    "is_active",
                    "is_deleted",
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
                    null,
                    "Bật máy bơm khi nhiệt độ vượt quá 30°C",
                    null,
                    "ALL",
                    true,
                    false,
                    new Guid("aaaaaaaa-0000-0000-0000-000000000902"),
                    30f,
                    null,
                    null,
                    "Kiểm soát nhiệt độ",
                    null,
                    new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                    null,
                }
            );

            migrationBuilder.InsertData(
                table: "sensor_logs",
                columns: new[]
                {
                    "id",
                    "created_at",
                    "data",
                    "data_json",
                    "is_warning",
                    "modified_at",
                    "sensor_id",
                },
                values: new object[,]
                {
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001401"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        28.5,
                        "{\"temperature\": 28.5, \"unit\": \"C\"}",
                        false,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001402"),
                        new DateTime(2024, 1, 1, 0, 30, 0, 0, DateTimeKind.Utc),
                        31.199999999999999,
                        "{\"temperature\": 31.2, \"unit\": \"C\"}",
                        true,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001301"),
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001403"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        7.2000000000000002,
                        "{\"ph\": 7.2}",
                        false,
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001302"),
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
                    "sensor_log_id",
                    "sensor_type_id",
                    "species_threshold_id",
                    "status",
                    "value",
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
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001402"),
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000501"),
                        "OPEN",
                        31.2f,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001802"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                        null,
                        new DateTime(2024, 1, 16, 8, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 1, 16, 10, 30, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001403"),
                        new Guid("eeeeeeee-0000-0000-0000-000000000002"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000502"),
                        "RESOLVED",
                        7.2f,
                    },
                    {
                        new Guid("aaaaaaaa-0000-0000-0000-000000001803"),
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                        new Guid("aaaaaaaa-0000-0000-0000-000000001501"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000301"),
                        null,
                        new DateTime(2024, 1, 17, 12, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        new Guid("aaaaaaaa-0000-0000-0000-000000001401"),
                        new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                        new Guid("aaaaaaaa-0000-0000-0000-000000000501"),
                        "ACKNOWLEDGED",
                        28.5f,
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "cameras",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000701")
            );

            migrationBuilder.DeleteData(
                table: "cameras",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000702")
            );

            migrationBuilder.DeleteData(
                table: "cameras",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000703")
            );

            migrationBuilder.DeleteData(
                table: "corrective_actions",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000002001")
            );

            migrationBuilder.DeleteData(
                table: "corrective_actions",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000002002")
            );

            migrationBuilder.DeleteData(
                table: "corrective_actions",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000002003")
            );

            migrationBuilder.DeleteData(
                table: "farming_batches",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001502")
            );

            migrationBuilder.DeleteData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001601")
            );

            migrationBuilder.DeleteData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001602")
            );

            migrationBuilder.DeleteData(
                table: "feeding_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001603")
            );

            migrationBuilder.DeleteData(
                table: "job_control_mappings",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001101")
            );

            migrationBuilder.DeleteData(
                table: "job_control_mappings",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001102")
            );

            migrationBuilder.DeleteData(
                table: "job_control_mappings",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001103")
            );

            migrationBuilder.DeleteData(
                table: "job_types",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000903")
            );

            migrationBuilder.DeleteData(
                table: "mortality_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001701")
            );

            migrationBuilder.DeleteData(
                table: "mortality_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001702")
            );

            migrationBuilder.DeleteData(
                table: "recommendations",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000002101")
            );

            migrationBuilder.DeleteData(
                table: "recommendations",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000002102")
            );

            migrationBuilder.DeleteData(
                table: "recommendations",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000002103")
            );

            migrationBuilder.DeleteData(
                table: "sensors",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001303")
            );

            migrationBuilder.DeleteData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001801")
            );

            migrationBuilder.DeleteData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001802")
            );

            migrationBuilder.DeleteData(
                table: "alerts",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001803")
            );

            migrationBuilder.DeleteData(
                table: "control_devices",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000801")
            );

            migrationBuilder.DeleteData(
                table: "control_devices",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000802")
            );

            migrationBuilder.DeleteData(
                table: "control_devices",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000803")
            );

            migrationBuilder.DeleteData(
                table: "documents",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001901")
            );

            migrationBuilder.DeleteData(
                table: "documents",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001902")
            );

            migrationBuilder.DeleteData(
                table: "documents",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001903")
            );

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001001")
            );

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001002")
            );

            migrationBuilder.DeleteData(
                table: "jobs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001003")
            );

            migrationBuilder.DeleteData(
                table: "master_boards",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001202")
            );

            migrationBuilder.DeleteData(
                table: "control_device_types",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000701")
            );

            migrationBuilder.DeleteData(
                table: "control_device_types",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000702")
            );

            migrationBuilder.DeleteData(
                table: "control_device_types",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000703")
            );

            migrationBuilder.DeleteData(
                table: "farming_batches",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001501")
            );

            migrationBuilder.DeleteData(
                table: "job_types",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000901")
            );

            migrationBuilder.DeleteData(
                table: "job_types",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000902")
            );

            migrationBuilder.DeleteData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001401")
            );

            migrationBuilder.DeleteData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001402")
            );

            migrationBuilder.DeleteData(
                table: "sensor_logs",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001403")
            );

            migrationBuilder.DeleteData(
                table: "sensors",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001301")
            );

            migrationBuilder.DeleteData(
                table: "sensors",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001302")
            );

            migrationBuilder.DeleteData(
                table: "master_boards",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000001201")
            );
        }
    }
}
