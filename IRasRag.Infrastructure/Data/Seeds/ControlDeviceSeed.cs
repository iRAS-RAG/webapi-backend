using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class ControlDeviceSeed
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

        public static readonly Guid Pump1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000801");

        public static List<ControlDevice> ControlDevices =>
            [
                new ControlDevice
                {
                    Id = Pump1Id,
                    Name = "Máy bơm nước",
                    PinCode = 6,
                    State = false,
                    CommandOn = "PUMP1_ON",
                    CommandOff = "PUMP1_OFF",
                    MasterBoardId = MasterBoardSeed.MasterBoardId,
                    ControlDeviceTypeId = ControlDeviceTypeSeed.PumpTypeId,
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
            ];
    }
}
