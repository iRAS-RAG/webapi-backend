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
                // Fingerling stage — 30 fish lost (~2 weeks into stage, ~15 g avg)
                new MortalityLog
                {
                    Id = MortalityLog1Id,
                    BatchId = FarmingBatchSeed.Batch1Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 30,
                    LostWeightKg = 0.45,
                    Date = new DateTime(2025, 09, 10, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Juvenile stage — 20 fish lost (~5 weeks into stage, ~100 g avg)
                new MortalityLog
                {
                    Id = MortalityLog2Id,
                    BatchId = FarmingBatchSeed.Batch1Id,
                    UserId = UserSeed.OperatorId,
                    Quantity = 20,
                    LostWeightKg = 2.0,
                    Date = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
            ];
    }
}
