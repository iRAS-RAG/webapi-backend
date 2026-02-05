using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class DocumentSeed
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

        public static readonly Guid Doc1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001901");

        public static readonly Guid Doc2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001902");

        public static readonly Guid Doc3Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001903");

        public static List<Document> Documents =>
            new()
            {
                new Document
                {
                    Id = Doc1Id,
                    Title = "Hướng dẫn xử lý nhiệt độ cao trong bể nuôi",
                    Content =
                        "Khi nhiệt độ nước vượt quá 30°C:\n1. Tăng lưu lượng nước tuần hoàn\n2. Bật hệ thống làm mát\n3. Giảm mật độ thả nuôi nếu cần\n4. Kiểm tra hàm lượng oxy hòa tan\n5. Theo dõi hành vi của cá",
                    UploadedByUserId = UserSeed.AdminId,
                    UploadedAt = new DateTime(2023, 12, 15, 10, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new Document
                {
                    Id = Doc2Id,
                    Title = "Quy trình điều chỉnh độ pH trong hệ thống RAS",
                    Content =
                        "Để duy trì độ pH ổn định:\n1. Kiểm tra độ kiềm của nước\n2. Sử dụng vôi nông nghiệp để tăng pH\n3. Sử dụng axit citric để giảm pH\n4. Theo dõi pH hàng ngày\n5. Đảm bảo hệ thống lọc sinh học hoạt động tốt",
                    UploadedByUserId = UserSeed.SupervisorId,
                    UploadedAt = new DateTime(2023, 12, 20, 14, 30, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new Document
                {
                    Id = Doc3Id,
                    Title = "Hướng dẫn quản lý chất lượng nước trong nuôi trồng thủy sản",
                    Content =
                        "Các thông số quan trọng cần theo dõi:\n1. Nhiệt độ: 25-30°C\n2. pH: 6.5-8.5\n3. Oxy hòa tan: >5 mg/L\n4. Ammonia: <0.1 mg/L\n5. Nitrite: <0.2 mg/L\n6. Nitrate: <50 mg/L\nThực hiện kiểm tra hàng ngày và ghi chép đầy đủ.",
                    UploadedByUserId = UserSeed.AdminId,
                    UploadedAt = new DateTime(2024, 01, 10, 9, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
