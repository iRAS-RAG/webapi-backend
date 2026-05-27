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

        public static readonly Guid TemperatureSensor1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001301"
        );

        public static readonly Guid PhSensor1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001302"
        );

        public static readonly Guid DoSensor1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001304"
        );

        public static List<Sensor> Sensors =>
            [
                new Sensor
                {
                    Id = TemperatureSensor1Id,
                    Name = "Cảm biến nhiệt độ 1",
                    PinCode = 2,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    MasterBoardId = MasterBoardSeed.MasterBoardId,
                    CreatedAt = SeedTimestamp,
                },
                new Sensor
                {
                    Id = PhSensor1Id,
                    Name = "Cảm biến pH 1",
                    PinCode = 3,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    MasterBoardId = MasterBoardSeed.MasterBoardId,
                    CreatedAt = SeedTimestamp,
                },
                new Sensor
                {
                    Id = DoSensor1Id,
                    Name = "Cảm biến oxy hòa tan 1",
                    PinCode = 4,
                    SensorTypeId = SensorTypeSeed.DoSensorTypeId,
                    MasterBoardId = MasterBoardSeed.MasterBoardId,
                    CreatedAt = SeedTimestamp,
                },
            ];
    }
}
