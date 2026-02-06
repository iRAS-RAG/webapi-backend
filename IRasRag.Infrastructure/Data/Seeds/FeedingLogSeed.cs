using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class FeedingLogSeed
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

        public static readonly Guid FeedLog1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001601");

        public static readonly Guid FeedLog2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001602");

        public static readonly Guid FeedLog3Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001603");

        public static List<FeedingLog> FeedingLogs =>
            new()
            {
                new FeedingLog
                {
                    Id = FeedLog1Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    Amount = 5.5f,
                    CreatedDate = new DateTime(2024, 01, 20, 6, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog2Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    Amount = 5.8f,
                    CreatedDate = new DateTime(2024, 01, 20, 12, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new FeedingLog
                {
                    Id = FeedLog3Id,
                    FarmingBatchId = FarmingBatchSeed.Batch1Id,
                    Amount = 6.0f,
                    CreatedDate = new DateTime(2024, 01, 20, 18, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
