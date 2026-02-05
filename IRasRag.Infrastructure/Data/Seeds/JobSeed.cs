using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class JobSeed
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

        public static readonly Guid MorningFeedingJobId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001001"
        );

        public static readonly Guid TemperatureControlJobId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001002"
        );

        public static readonly Guid AerationJobId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000001003"
        );

        public static List<Job> Jobs =>
            new()
            {
                new Job
                {
                    Id = MorningFeedingJobId,
                    Name = "Cho ăn buổi sáng",
                    Description = "Cho cá ăn tự động vào lúc 6 giờ sáng mỗi ngày",
                    JobTypeId = JobTypeSeed.ScheduledJobTypeId,
                    SensorId = null,
                    MinValue = null,
                    MaxValue = null,
                    DefaultState = false,
                    IsActive = true,
                    StartTime = new TimeSpan(6, 0, 0),
                    EndTime = new TimeSpan(6, 5, 0),
                    RepeatIntervalMinutes = null,
                    ExecutionDays = "ALL",
                    CreatedAt = SeedTimestamp,
                },
                new Job
                {
                    Id = TemperatureControlJobId,
                    Name = "Kiểm soát nhiệt độ",
                    Description = "Bật máy bơm khi nhiệt độ vượt quá 30°C",
                    JobTypeId = JobTypeSeed.SensorBasedJobTypeId,
                    SensorId = SensorSeed.TemperatureSensor1Id,
                    MinValue = null,
                    MaxValue = 30.0f,
                    DefaultState = false,
                    IsActive = true,
                    StartTime = null,
                    EndTime = null,
                    RepeatIntervalMinutes = null,
                    ExecutionDays = "ALL",
                    CreatedAt = SeedTimestamp,
                },
                new Job
                {
                    Id = AerationJobId,
                    Name = "Sục khí định kỳ",
                    Description = "Sục khí 15 phút mỗi 3 giờ trong giờ làm việc",
                    JobTypeId = JobTypeSeed.ScheduledJobTypeId,
                    SensorId = null,
                    MinValue = null,
                    MaxValue = null,
                    DefaultState = false,
                    IsActive = true,
                    StartTime = new TimeSpan(6, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    RepeatIntervalMinutes = 180,
                    ExecutionDays = "ALL",
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
