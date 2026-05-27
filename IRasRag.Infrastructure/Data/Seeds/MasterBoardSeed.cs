using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class MasterBoardSeed
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

        public static readonly Guid MasterBoardId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001201"
        );

        public static List<MasterBoard> MasterBoards =>
            [
                new MasterBoard
                {
                    Id = MasterBoardId,
                    Name = "Board điều khiển chính 1",
                    MacAddress = "AA:BB:CC:DD:EE:01",
                    FishTankId = FishTankSeed.TankAId,
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
            ];
    }
}
