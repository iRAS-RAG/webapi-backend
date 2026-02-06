using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class MasterBoardSeed
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

        public static readonly Guid MasterBoard1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001201"
        );

        public static readonly Guid MasterBoard2Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001202"
        );

        public static List<MasterBoard> MasterBoards =>
            new()
            {
                new MasterBoard
                {
                    Id = MasterBoard1Id,
                    Name = "Board điều khiển chính 1",
                    MacAddress = "AA:BB:CC:DD:EE:01",
                    FishTankId = FishTankSeed.TankAId,
                    CreatedAt = SeedTimestamp,
                },
                new MasterBoard
                {
                    Id = MasterBoard2Id,
                    Name = "Board điều khiển chính 2",
                    MacAddress = "AA:BB:CC:DD:EE:02",
                    FishTankId = FishTankSeed.TankAId,
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
