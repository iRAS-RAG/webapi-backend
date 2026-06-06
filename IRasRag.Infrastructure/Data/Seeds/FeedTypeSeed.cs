using IRasRag.Domain.Entities;

public static class FeedTypeSeed
{
    public static readonly Guid StarterFeedId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000201");

    public static readonly Guid GrowerFeedId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000202");

    public static readonly Guid FinisherFeedId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000203");

    public static readonly Guid CrabStarterFeedId = Guid.Parse(
        "aaaaaaaa-0000-0000-0000-000000000204"
    );

    public static readonly Guid CrabGrowerFeedId = Guid.Parse(
        "aaaaaaaa-0000-0000-0000-000000000205"
    );

    public static readonly Guid SquidStarterFeedId = Guid.Parse(
        "aaaaaaaa-0000-0000-0000-000000000206"
    );

    public static readonly Guid SquidGrowerFeedId = Guid.Parse(
        "aaaaaaaa-0000-0000-0000-000000000207"
    );

    public static List<FeedType> FeedTypes =>
        [
            new FeedType
            {
                Id = StarterFeedId,
                Name = "Giàu protein",
                Description =
                    "Thức ăn dạng bột mịn, hàm lượng protein cao (≥40%), "
                    + "phù hợp cho giai đoạn cá bột.",
                ProteinPercentage = 45.0,
                Manufacturer = "Grobest Vietnam",
                CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
            },
            new FeedType
            {
                Id = GrowerFeedId,
                Name = "Tiêu chuẩn",
                Description =
                    "Thức ăn viên nhỏ, hàm lượng protein trung bình (35–40%), "
                    + "phù hợp cho giai đoạn cá hương và cá giống.",
                ProteinPercentage = 38.0,
                Manufacturer = "Cargill Vietnam",
                CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
            },
            new FeedType
            {
                Id = FinisherFeedId,
                Name = "Thương phẩm",
                Description =
                    "Thức ăn viên lớn, hàm lượng protein vừa phải (28–32%), "
                    + "phù hợp cho giai đoạn cá thương phẩm đến khi thu hoạch.",
                ProteinPercentage = 30.0,
                Manufacturer = "Uni-President Vietnam",
                CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
            },
        ];
}
