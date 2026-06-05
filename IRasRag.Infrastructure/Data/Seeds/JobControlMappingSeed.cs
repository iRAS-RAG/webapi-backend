using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class JobControlMappingSeed
    {
        private static readonly DateTime SeedTimestamp = new(
            2024,
            01,
            01,
            0,
            0,
            0,
            DateTimeKind.Utc
        );

        public static readonly Guid Mapping1Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000001101");

        public static List<JobControlMapping> JobControlMappings =>
            [
                new JobControlMapping
                {
                    Id = Mapping1Id,
                    JobId = JobSeed.TemperatureControlJobId,
                    ControlDeviceId = ControlDeviceSeed.Pump1Id,
                    TargetState = true,
                    TriggerCondition = JobTriggerCondition.ABOVE_MAX,
                    CreatedAt = SeedTimestamp,
                },
            ];
    }
}
