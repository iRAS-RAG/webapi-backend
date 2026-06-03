using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class FeedingLogSeed
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

        public static readonly Guid FeedLog1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001601");

        public static readonly Guid FeedLog2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001602");

        public static readonly Guid FeedLog3Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001603");

        public static List<FeedingLog> FeedingLogs =>
            [
                // Fry stage — starter feed, morning (0.3 kg/100fish × 9.8 ≈ 2.9 kg)
                new FeedingLog
                {
                    Id = FeedLog1Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 3.0,
                    CreatedDate = new DateTime(2025, 08, 10, 6, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Fry stage — starter feed, midday
                new FeedingLog
                {
                    Id = FeedLog2Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.StarterFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 2.8,
                    CreatedDate = new DateTime(2025, 08, 14, 12, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                // Fingerling stage — grower feed, evening (1.0 kg/100fish × 9.5 ≈ 9.5 kg)
                new FeedingLog
                {
                    Id = FeedLog3Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    FeedTypeId = FeedTypeSeed.GrowerFeedId,
                    UserId = UserSeed.OperatorId,
                    Amount = 9.0,
                    CreatedDate = new DateTime(2025, 09, 05, 18, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
            ];
    }
}
