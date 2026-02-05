using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class JobTypeSeed
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

        public static readonly Guid ScheduledJobTypeId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000901"
        );

        public static readonly Guid SensorBasedJobTypeId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000902"
        );

        public static readonly Guid ManualJobTypeId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000903"
        );

        public static List<JobType> JobTypes =>
            new()
            {
                new JobType
                {
                    Id = ScheduledJobTypeId,
                    Name = "Công việc theo lịch",
                    Description = "Tự động thực hiện theo thời gian đã định",
                    CreatedAt = SeedTimestamp,
                },
                new JobType
                {
                    Id = SensorBasedJobTypeId,
                    Name = "Công việc dựa trên cảm biến",
                    Description = "Tự động thực hiện dựa trên giá trị cảm biến",
                    CreatedAt = SeedTimestamp,
                },
                new JobType
                {
                    Id = ManualJobTypeId,
                    Name = "Công việc thủ công",
                    Description = "Thực hiện bằng tay khi cần thiết",
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
