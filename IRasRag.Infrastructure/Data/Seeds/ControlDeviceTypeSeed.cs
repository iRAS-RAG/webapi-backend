using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class ControlDeviceTypeSeed
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

        public static readonly Guid PumpTypeId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000701");

        public static readonly Guid AeratorTypeId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000702"
        );

        public static readonly Guid FeederTypeId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000703"
        );

        public static List<ControlDeviceType> ControlDeviceTypes =>
            [
                new ControlDeviceType
                {
                    Id = PumpTypeId,
                    Name = "Máy bơm nước",
                    Description = "Thiết bị bơm nước tuần hoàn trong hệ thống RAS",
                    CreatedAt = SeedTimestamp,
                },
            ];
    }
}
