using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class SpeciesThresholdSeed
    {
        public static List<SpeciesThreshold> SpeciesThresholds =>
            [
                // ═══════════════════════════════════════════════════════
                // Cá rô phi – Giai đoạn Cá bột (Fry)
                // ═══════════════════════════════════════════════════════

                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000501"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FryStageId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    MinValue = 27.0,
                    MaxValue = 29.0,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000502"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FryStageId,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    MinValue = 6.8,
                    MaxValue = 8.2,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000503"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FryStageId,
                    SensorTypeId = SensorTypeSeed.TdsSensorTypeId,
                    MinValue = 250,
                    MaxValue = 400,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000504"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FryStageId,
                    SensorTypeId = SensorTypeSeed.FlowSensorTypeId,
                    MinValue = 30,
                    MaxValue = 45,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000505"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FryStageId,
                    SensorTypeId = SensorTypeSeed.WaterLevelSensorTypeId,
                    MinValue = 1,
                    MaxValue = 1,
                },
                // ═══════════════════════════════════════════════════════
                // Cá rô phi – Giai đoạn Cá hương (Fingerling)
                // ═══════════════════════════════════════════════════════

                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000509"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FingerlingStageId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    MinValue = 26.5,
                    MaxValue = 29.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000510"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FingerlingStageId,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    MinValue = 6.5,
                    MaxValue = 8.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000511"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FingerlingStageId,
                    SensorTypeId = SensorTypeSeed.TdsSensorTypeId,
                    MinValue = 200,
                    MaxValue = 400,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000512"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FingerlingStageId,
                    SensorTypeId = SensorTypeSeed.FlowSensorTypeId,
                    MinValue = 35,
                    MaxValue = 50,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000513"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FingerlingStageId,
                    SensorTypeId = SensorTypeSeed.WaterLevelSensorTypeId,
                    MinValue = 1,
                    MaxValue = 1,
                },
                // ═══════════════════════════════════════════════════════
                // Cá rô phi – Giai đoạn Cá giống (Juvenile)
                // ═══════════════════════════════════════════════════════

                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000517"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.JuvenileStageId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    MinValue = 26.0,
                    MaxValue = 30.0,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000518"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.JuvenileStageId,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    MinValue = 6.5,
                    MaxValue = 8.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000519"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.JuvenileStageId,
                    SensorTypeId = SensorTypeSeed.TdsSensorTypeId,
                    MinValue = 200,
                    MaxValue = 450,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000520"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.JuvenileStageId,
                    SensorTypeId = SensorTypeSeed.FlowSensorTypeId,
                    MinValue = 40,
                    MaxValue = 55,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000521"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.JuvenileStageId,
                    SensorTypeId = SensorTypeSeed.WaterLevelSensorTypeId,
                    MinValue = 1,
                    MaxValue = 1,
                },
                // ═══════════════════════════════════════════════════════
                // Cá rô phi – Giai đoạn Cá thương phẩm (Grow-out)
                // ═══════════════════════════════════════════════════════

                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000525"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.GrowOutStageId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    MinValue = 26.0,
                    MaxValue = 30.0,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000526"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.GrowOutStageId,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    MinValue = 6.5,
                    MaxValue = 8.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000527"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.GrowOutStageId,
                    SensorTypeId = SensorTypeSeed.TdsSensorTypeId,
                    MinValue = 200,
                    MaxValue = 450,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000528"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.GrowOutStageId,
                    SensorTypeId = SensorTypeSeed.FlowSensorTypeId,
                    MinValue = 40,
                    MaxValue = 55,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000529"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.GrowOutStageId,
                    SensorTypeId = SensorTypeSeed.WaterLevelSensorTypeId,
                    MinValue = 1,
                    MaxValue = 1,
                },
                // ═══════════════════════════════════════════════════════
                // Cua biển – Giai đoạn Cua bột (Fry)
                // ═══════════════════════════════════════════════════════

                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000533"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabFryStageId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    MinValue = 27.5,
                    MaxValue = 30.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000534"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabFryStageId,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    MinValue = 7.8,
                    MaxValue = 8.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000535"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabFryStageId,
                    SensorTypeId = SensorTypeSeed.TdsSensorTypeId,
                    MinValue = 250,
                    MaxValue = 400,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000536"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabFryStageId,
                    SensorTypeId = SensorTypeSeed.FlowSensorTypeId,
                    MinValue = 25,
                    MaxValue = 40,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000537"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabFryStageId,
                    SensorTypeId = SensorTypeSeed.WaterLevelSensorTypeId,
                    MinValue = 1,
                    MaxValue = 1,
                },
                // ═══════════════════════════════════════════════════════
                // Cua biển – Giai đoạn Cua giống (Juvenile)
                // ═══════════════════════════════════════════════════════

                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000541"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    MinValue = 27.0,
                    MaxValue = 31.0,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000542"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    MinValue = 7.5,
                    MaxValue = 8.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000543"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.TdsSensorTypeId,
                    MinValue = 200,
                    MaxValue = 400,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000544"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.FlowSensorTypeId,
                    MinValue = 30,
                    MaxValue = 50,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000545"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.WaterLevelSensorTypeId,
                    MinValue = 1,
                    MaxValue = 1,
                },
                // ═══════════════════════════════════════════════════════
                // Cua biển – Giai đoạn Cua thịt (Grow-out)
                // ═══════════════════════════════════════════════════════

                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000549"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    MinValue = 26.0,
                    MaxValue = 31.0,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000550"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    MinValue = 7.5,
                    MaxValue = 8.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000551"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.TdsSensorTypeId,
                    MinValue = 200,
                    MaxValue = 450,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000552"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.FlowSensorTypeId,
                    MinValue = 35,
                    MaxValue = 55,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000553"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.WaterLevelSensorTypeId,
                    MinValue = 1,
                    MaxValue = 1,
                },
                // ═══════════════════════════════════════════════════════
                // Mực lá – Giai đoạn Mực ấu trùng (Paralarva)
                // ═══════════════════════════════════════════════════════

                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000557"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidParalarvaStageId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    MinValue = 25.0,
                    MaxValue = 27.0,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000558"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidParalarvaStageId,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    MinValue = 8.0,
                    MaxValue = 8.3,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000559"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidParalarvaStageId,
                    SensorTypeId = SensorTypeSeed.TdsSensorTypeId,
                    MinValue = 350,
                    MaxValue = 500,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000560"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidParalarvaStageId,
                    SensorTypeId = SensorTypeSeed.FlowSensorTypeId,
                    MinValue = 30,
                    MaxValue = 45,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000561"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidParalarvaStageId,
                    SensorTypeId = SensorTypeSeed.WaterLevelSensorTypeId,
                    MinValue = 1,
                    MaxValue = 1,
                },
                // ═══════════════════════════════════════════════════════
                // Mực lá – Giai đoạn Mực non (Juvenile)
                // ═══════════════════════════════════════════════════════

                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000565"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    MinValue = 24.0,
                    MaxValue = 28.0,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000566"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    MinValue = 7.8,
                    MaxValue = 8.3,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000567"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.TdsSensorTypeId,
                    MinValue = 300,
                    MaxValue = 500,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000568"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.FlowSensorTypeId,
                    MinValue = 35,
                    MaxValue = 50,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000569"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.WaterLevelSensorTypeId,
                    MinValue = 1,
                    MaxValue = 1,
                },
                // ═══════════════════════════════════════════════════════
                // Mực lá – Giai đoạn Mực thương phẩm (Grow-out)
                // ═══════════════════════════════════════════════════════

                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000573"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.TemperatureSensorTypeId,
                    MinValue = 24.0,
                    MaxValue = 28.0,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000574"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.PhSensorTypeId,
                    MinValue = 7.8,
                    MaxValue = 8.3,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000575"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.TdsSensorTypeId,
                    MinValue = 300,
                    MaxValue = 500,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000576"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.FlowSensorTypeId,
                    MinValue = 40,
                    MaxValue = 55,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000577"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.WaterLevelSensorTypeId,
                    MinValue = 1,
                    MaxValue = 1,
                },
            ];
    }
}
