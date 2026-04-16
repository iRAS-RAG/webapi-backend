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
            new()
            {
                new SensorType
                {
                    Id = TemperatureSensorTypeId,
                    Name = "Nhiệt độ nước",
                    MeasureType = "Nhiệt độ",
                    UnitOfMeasure = "Độ C",
                },
                new SensorType
                {
                    Id = PhSensorTypeId,
                    Name = "Độ pH",
                    MeasureType = "Tính axit",
                    UnitOfMeasure = "pH",
                },
                new SensorType
                {
                    Id = TdsSensorTypeId,
                    Name = "TDS",
                    MeasureType = "Tổng chất rắn hòa tan",
                    UnitOfMeasure = "ppm",
                },
                new SensorType
                {
                    Id = FlowSensorTypeId,
                    Name = "Lưu lượng nước",
                    MeasureType = "Lưu lượng",
                    UnitOfMeasure = "L/min",
                },
                new SensorType
                {
                    Id = WaterLevelSensorTypeId,
                    Name = "Mực nước",
                    MeasureType = "Mức nước",
                    UnitOfMeasure = "0/1",
                },
            };
    }
}
