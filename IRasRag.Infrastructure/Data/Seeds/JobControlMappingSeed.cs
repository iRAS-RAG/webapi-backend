using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class JobControlMappingSeed
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

        public static readonly Guid Mapping1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001101");

        public static readonly Guid Mapping2Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001102");

        public static readonly Guid Mapping3Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001103");

        public static List<JobControlMapping> JobControlMappings =>
            new()
            {
                new JobControlMapping
                {
                    Id = Mapping1Id,
                    JobId = JobSeed.MorningFeedingJobId,
                    ControlDeviceId = ControlDeviceSeed.Feeder1Id,
                    TargetState = true,
                    TriggerCondition = JobTriggerCondition.ALWAYS,
                    CreatedAt = SeedTimestamp,
                },
                new JobControlMapping
                {
                    Id = Mapping2Id,
                    JobId = JobSeed.TemperatureControlJobId,
                    ControlDeviceId = ControlDeviceSeed.Pump1Id,
                    TargetState = true,
                    TriggerCondition = JobTriggerCondition.ABOVE_MAX,
                    CreatedAt = SeedTimestamp,
                },
                new JobControlMapping
                {
                    Id = Mapping3Id,
                    JobId = JobSeed.AerationJobId,
                    ControlDeviceId = ControlDeviceSeed.Aerator1Id,
                    TargetState = true,
                    TriggerCondition = JobTriggerCondition.ALWAYS,
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
