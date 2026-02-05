using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class RecommendationSeed
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

        public static readonly Guid Rec1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000002101");

        public static readonly Guid Rec2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000002102");

        public static readonly Guid Rec3Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000002103");

        public static List<Recommendation> Recommendations =>
            new()
            {
                new Recommendation
                {
                    Id = Rec1Id,
                    AlertId = AlertSeed.Alert1Id,
                    DocumentId = DocumentSeed.Doc1Id,
                    SuggestionText =
                        "Áp dụng quy trình xử lý nhiệt độ cao trong tài liệu: Tăng lưu lượng nước tuần hoàn và bật hệ thống làm mát. Kiểm tra mức oxy hòa tan để đảm bảo cá không bị thiếu oxy.",
                    CreatedAt = SeedTimestamp,
                },
                new Recommendation
                {
                    Id = Rec2Id,
                    AlertId = AlertSeed.Alert2Id,
                    DocumentId = DocumentSeed.Doc2Id,
                    SuggestionText =
                        "Theo quy trình điều chỉnh pH: Thêm vôi nông nghiệp để tăng độ pH lên mức tối ưu (7.5-8.0). Theo dõi pH hàng ngày và điều chỉnh nếu cần.",
                    CreatedAt = SeedTimestamp,
                },
                new Recommendation
                {
                    Id = Rec3Id,
                    AlertId = AlertSeed.Alert3Id,
                    DocumentId = DocumentSeed.Doc3Id,
                    SuggestionText =
                        "Tham khảo hướng dẫn quản lý chất lượng nước: Duy trì nhiệt độ trong khoảng 25-30°C. Kiểm tra các thông số khác như oxy hòa tan, ammonia và nitrite để đảm bảo môi trường nuôi tối ưu.",
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
