using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class SensorTypeSeed
    {
        public static readonly Guid TemperatureSensorTypeId = Guid.Parse(
            "eeeeeeee-0000-0000-0000-000000000001"
        );

        public static readonly Guid PhSensorTypeId = Guid.Parse(
            "eeeeeeee-0000-0000-0000-000000000002"
        );

        public static readonly Guid TdsSensorTypeId = Guid.Parse(
            "eeeeeeee-0000-0000-0000-000000000003"
        );

        public static readonly Guid FlowSensorTypeId = Guid.Parse(
            "eeeeeeee-0000-0000-0000-000000000004"
        );

        public static readonly Guid WaterLevelSensorTypeId = Guid.Parse(
            "eeeeeeee-0000-0000-0000-000000000005"
        );

        // Backward compatibility for existing seed references.
        public static readonly Guid DoSensorTypeId = TdsSensorTypeId;

        public static List<SensorType> SensorTypes =>
            [
                new SensorType
                {
                    Id = TemperatureSensorTypeId,
                    Name = "Nhiệt độ nước",
                    MeasureType = "Nhiệt độ",
                    UnitOfMeasure = "Độ C",
                    Code = "waterTemp",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                new SensorType
                {
                    Id = PhSensorTypeId,
                    Name = "Độ pH",
                    MeasureType = "Tính axit",
                    UnitOfMeasure = "pH",
                    Code = "pH",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                new SensorType
                {
                    Id = TdsSensorTypeId,
                    Name = "TDS",
                    MeasureType = "Tổng chất rắn hòa tan",
                    UnitOfMeasure = "ppm",
                    Code = "tds",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                new SensorType
                {
                    Id = FlowSensorTypeId,
                    Name = "Lưu lượng nước",
                    MeasureType = "Lưu lượng",
                    UnitOfMeasure = "L/min",
                    Code = "flowRate",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                new SensorType
                {
                    Id = WaterLevelSensorTypeId,
                    Name = "Mực nước",
                    MeasureType = "Mức nước",
                    UnitOfMeasure = "0/1",
                    Code = "waterLevel",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
            ];
    }
}
