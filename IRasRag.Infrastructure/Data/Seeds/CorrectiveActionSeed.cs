using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class CorrectiveActionSeed
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

        public static readonly Guid Action1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000002001");

        public static readonly Guid Action2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000002002");

        public static readonly Guid Action3Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000002003");

        public static List<CorrectiveAction> CorrectiveActions =>
            new()
            {
                new CorrectiveAction
                {
                    Id = Action1Id,
                    AlertId = AlertSeed.Alert1Id,
                    UserId = UserSeed.OperatorId,
                    ActionTaken = "Bật hệ thống làm mát và tăng lưu lượng nước tuần hoàn",
                    Notes = "Nhiệt độ giảm từ 31.2°C xuống 29.5°C sau 2 giờ. Tiếp tục theo dõi.",
                    Timestamp = new DateTime(2024, 01, 15, 15, 0, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new CorrectiveAction
                {
                    Id = Action2Id,
                    AlertId = AlertSeed.Alert2Id,
                    UserId = UserSeed.SupervisorId,
                    ActionTaken = "Thêm vôi nông nghiệp để điều chỉnh pH lên mức tối ưu",
                    Notes =
                        "Đã thêm 500g vôi. pH tăng từ 7.2 lên 7.6 sau 1 giờ. Vấn đề đã được giải quyết.",
                    Timestamp = new DateTime(2024, 01, 16, 9, 15, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
                new CorrectiveAction
                {
                    Id = Action3Id,
                    AlertId = AlertSeed.Alert3Id,
                    UserId = UserSeed.OperatorId,
                    ActionTaken = "Kiểm tra hệ thống sục khí và làm sạch bộ lọc",
                    Notes =
                        "Đã vệ sinh bộ lọc cơ học. Hệ thống sục khí hoạt động bình thường. Nhiệt độ ổn định.",
                    Timestamp = new DateTime(2024, 01, 17, 13, 30, 0, DateTimeKind.Utc),
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
