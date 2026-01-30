using IRasRag.Domain.Entities;

public static class FeedTypeSeed
{
    public static readonly Guid StarterFeedId =
        Guid.Parse("aaaaaaaa-0000-0000-0000-000000000201");

    public static readonly Guid GrowerFeedId =
        Guid.Parse("aaaaaaaa-0000-0000-0000-000000000202");

    public static List<FeedType> FeedTypes =>
        new()
        {
            new FeedType
            {
                Id = StarterFeedId,
                Name = "Giàu protein",
                WeightPerUnit = 25.0f,
                Description = "Thức ăn có hàm lượng protein cao, phù hợp cho giai đoạn đầu phát triển của cá.",
                ProteinPercentage = 45.0f,
                Manufacturer = "AquaFeed Solutions"
            },
            new FeedType
            {
                Id = GrowerFeedId,
                Name = "Tiêu chuẩn",
                WeightPerUnit = 25.0f,
                Description = "Thức ăn tiêu chuẩn, phù hợp cho giai đoạn phát triển tiếp theo của cá.",
                ProteinPercentage = 38.0f,
                Manufacturer = "AquaFeed Solutions"
            }
        };
}
