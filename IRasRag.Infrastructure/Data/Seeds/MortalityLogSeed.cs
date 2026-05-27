using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class MortalityLogSeed
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

        public static readonly Guid MortalityLog1Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001701"
        );

        public static readonly Guid MortalityLog2Id = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001702"
        );

        public static List<MortalityLog> MortalityLogs =>
            [
                new MortalityLog
                {
                    Id = MortalityLog1Id,
                    BatchId = FarmingBatchSeed.Batch1Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 30,
                    LostWeightKg = 4.5,
                    Date = new DateTime(2024, 02, 15, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new MortalityLog
                {
                    Id = MortalityLog2Id,
                    BatchId = FarmingBatchSeed.Batch1Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 20,
                    LostWeightKg = 3.0,
                    Date = new DateTime(2024, 03, 10, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
            ];
    }
}
