using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class SensorTypeSeed
    {
        public static readonly Guid TemperatureSensorTypeId =
            Guid.Parse("eeeeeeee-0000-0000-0000-000000000001");

        public static readonly Guid PhSensorTypeId =
            Guid.Parse("eeeeeeee-0000-0000-0000-000000000002");

        public static List<SensorType> SensorTypes =>
            new()
            {
            new SensorType
            {
                Id = TemperatureSensorTypeId,
                Name = "Nhiệt độ"
            },
            new SensorType
            {
                Id = PhSensorTypeId,
                Name = "Độ pH"
            }
            };
    }

}
