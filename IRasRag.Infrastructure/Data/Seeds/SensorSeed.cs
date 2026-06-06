using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class SensorSeed
    {
        private static readonly DateTime SeedTimestamp = new(
            2024,
            01,
            01,
            0,
            0,
            0,
            DateTimeKind.Utc
        );

        // Pin 1 – Temperature
        public static readonly Guid TemperatureSensor1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001301"
        );

        // Pin 2 – pH
        public static readonly Guid PhSensor1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001302"
        );

        // Pin 3 – TDS
        public static readonly Guid TdsSensor1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001303"
        );

        // Pin 4 – Water Flow
        public static readonly Guid FlowSensor1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001304"
        );

        // Pin 5 – Water Level
        public static readonly Guid WaterLevelSensor1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001305"
        );

        // Pin 6 – Voltage
        public static readonly Guid VoltageSensor1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001306"
        );

        // Pin 7 – Current
        public static readonly Guid CurrentSensor1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001307"
        );

        // Pin 8 – PZEM Power
        public static readonly Guid PowerSensor1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001308"
        );

        // Backward-compatible alias for the old DO sensor ID (now pin 4 – Water Flow).
        public static readonly Guid DoSensor1Id = FlowSensor1Id;

        public static List<Sensor> Sensors =>
            [
                new Sensor
                {
                    Id = TemperatureSensor1Id,
                    Name = "Cảm biến nhiệt độ 1",
                    PinCode = 1,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    MasterBoardId = MasterBoardSeed.MasterBoardId,
                    CreatedAt = SeedTimestamp,
                },
                new Sensor
                {
                    Id = PhSensor1Id,
                    Name = "Cảm biến pH 1",
                    PinCode = 2,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    MasterBoardId = MasterBoardSeed.MasterBoardId,
                    CreatedAt = SeedTimestamp,
                },
                new Sensor
                {
                    Id = TdsSensor1Id,
                    Name = "Cảm biến TDS 1",
                    PinCode = 3,
                    SensorTypeId = SensorTypeSeed.TdsSensorTypeId,
                    MasterBoardId = MasterBoardSeed.MasterBoardId,
                    CreatedAt = SeedTimestamp,
                },
                new Sensor
                {
                    Id = FlowSensor1Id,
                    Name = "Cảm biến lưu lượng nước 1",
                    PinCode = 4,
                    SensorTypeId = SensorTypeSeed.FlowSensorTypeId,
                    MasterBoardId = MasterBoardSeed.MasterBoardId,
                    CreatedAt = SeedTimestamp,
                },
                new Sensor
                {
                    Id = WaterLevelSensor1Id,
                    Name = "Cảm biến mực nước 1",
                    PinCode = 5,
                    SensorTypeId = SensorTypeSeed.WaterLevelSensorTypeId,
                    MasterBoardId = MasterBoardSeed.MasterBoardId,
                    CreatedAt = SeedTimestamp,
                },
            ];
    }
}
