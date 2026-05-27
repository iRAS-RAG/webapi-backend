using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class SpeciesThresholdSeed
    {
        public static List<SpeciesThreshold> SpeciesThresholds =>
            [
                // ============================
                // Cá rô phi – Giai đoạn cá bột
                // ============================

                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000501"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FryStageId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    MinValue = 26,
                    MaxValue = 30,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000502"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FryStageId,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    MinValue = 6.5,
                    MaxValue = 8.0,
                },
                // ============================
                // Cá rô phi – Giai đoạn cá giống
                // ============================

                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000503"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.JuvenileStageId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    MinValue = 25,
                    MaxValue = 29,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000504"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.JuvenileStageId,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    MinValue = 6.5,
                    MaxValue = 8.5,
                },
            ];
    }
}
