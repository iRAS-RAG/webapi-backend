using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class ControlDeviceSeed
    {
        private static readonly DateTime SeedTimestamp = new DateTime(
            2024,
            01,
            01,
            0,
            0,
            0,
            DateTimeKind.Utc
        );

        public static readonly Guid Pump1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000801");

        public static readonly Guid Aerator1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000802");

        public static readonly Guid Feeder1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000803");

        public static List<ControlDevice> ControlDevices =>
            new()
            {
                new ControlDevice
                {
                    Id = Pump1Id,
                    Name = "Máy bơm chính 1",
                    PinCode = 5,
                    State = false,
                    CommandOn = "PUMP1_ON",
                    CommandOff = "PUMP1_OFF",
                    MasterBoardId = MasterBoardSeed.MasterBoard1Id,
                    ControlDeviceTypeId = ControlDeviceTypeSeed.PumpTypeId,
                    CreatedAt = SeedTimestamp,
                },
                new ControlDevice
                {
                    Id = Aerator1Id,
                    Name = "Máy sục khí 1",
                    PinCode = 6,
                    State = false,
                    CommandOn = "AERATOR1_ON",
                    CommandOff = "AERATOR1_OFF",
                    MasterBoardId = MasterBoardSeed.MasterBoard1Id,
                    ControlDeviceTypeId = ControlDeviceTypeSeed.AeratorTypeId,
                    CreatedAt = SeedTimestamp,
                },
                new ControlDevice
                {
                    Id = Feeder1Id,
                    Name = "Máy cho ăn tự động 1",
                    PinCode = 7,
                    State = false,
                    CommandOn = "FEEDER1_ON",
                    CommandOff = "FEEDER1_OFF",
                    MasterBoardId = MasterBoardSeed.MasterBoard1Id,
                    ControlDeviceTypeId = ControlDeviceTypeSeed.FeederTypeId,
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
