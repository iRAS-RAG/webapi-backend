using IRasRag.Domain.Entities;

public static class FeedTypeSeed
{
    public static readonly Guid StarterFeedId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000201");

    public static readonly Guid GrowerFeedId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000202");

    public static List<FeedType> FeedTypes =>
        [
            new FeedType
            {
                Id = StarterFeedId,
                Name = "Giàu protein",
                Description =
                    "Thức ăn có hàm lượng protein cao, phù hợp cho giai đoạn đầu phát triển của cá.",
                ProteinPercentage = 45.0,
                Manufacturer = "AquaFeed Solutions",
                CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
            },
            new FeedType
            {
                Id = GrowerFeedId,
                Name = "Tiêu chuẩn",
                Description =
                    "Thức ăn tiêu chuẩn, phù hợp cho giai đoạn phát triển tiếp theo của cá.",
                ProteinPercentage = 38.0,
                Manufacturer = "AquaFeed Solutions",
                CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
            },
        ];
}
