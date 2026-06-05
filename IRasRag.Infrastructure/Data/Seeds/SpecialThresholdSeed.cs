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
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000506"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FryStageId,
                    SensorTypeId = SensorTypeSeed.VoltageSensorTypeId,
                    MinValue = 11.5,
                    MaxValue = 12.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000507"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FryStageId,
                    SensorTypeId = SensorTypeSeed.CurrentSensorTypeId,
                    MinValue = 0.5,
                    MaxValue = 1.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000508"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FryStageId,
                    SensorTypeId = SensorTypeSeed.PowerSensorTypeId,
                    MinValue = 20,
                    MaxValue = 40,
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
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000514"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FingerlingStageId,
                    SensorTypeId = SensorTypeSeed.VoltageSensorTypeId,
                    MinValue = 11.5,
                    MaxValue = 12.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000515"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FingerlingStageId,
                    SensorTypeId = SensorTypeSeed.CurrentSensorTypeId,
                    MinValue = 0.5,
                    MaxValue = 2.0,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000516"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.FingerlingStageId,
                    SensorTypeId = SensorTypeSeed.PowerSensorTypeId,
                    MinValue = 25,
                    MaxValue = 45,
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
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000522"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.JuvenileStageId,
                    SensorTypeId = SensorTypeSeed.VoltageSensorTypeId,
                    MinValue = 11.5,
                    MaxValue = 12.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000523"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.JuvenileStageId,
                    SensorTypeId = SensorTypeSeed.CurrentSensorTypeId,
                    MinValue = 0.8,
                    MaxValue = 2.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000524"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.JuvenileStageId,
                    SensorTypeId = SensorTypeSeed.PowerSensorTypeId,
                    MinValue = 30,
                    MaxValue = 50,
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
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000530"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.GrowOutStageId,
                    SensorTypeId = SensorTypeSeed.VoltageSensorTypeId,
                    MinValue = 11.5,
                    MaxValue = 12.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000531"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.GrowOutStageId,
                    SensorTypeId = SensorTypeSeed.CurrentSensorTypeId,
                    MinValue = 1.0,
                    MaxValue = 3.0,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000532"),
                    SpeciesId = SpeciesSeed.TilapiaId,
                    GrowthStageId = GrowthStageSeed.GrowOutStageId,
                    SensorTypeId = SensorTypeSeed.PowerSensorTypeId,
                    MinValue = 30,
                    MaxValue = 60,
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
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000538"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabFryStageId,
                    SensorTypeId = SensorTypeSeed.VoltageSensorTypeId,
                    MinValue = 11.5,
                    MaxValue = 12.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000539"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabFryStageId,
                    SensorTypeId = SensorTypeSeed.CurrentSensorTypeId,
                    MinValue = 0.5,
                    MaxValue = 1.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000540"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabFryStageId,
                    SensorTypeId = SensorTypeSeed.PowerSensorTypeId,
                    MinValue = 20,
                    MaxValue = 35,
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
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000546"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.VoltageSensorTypeId,
                    MinValue = 11.5,
                    MaxValue = 12.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000547"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.CurrentSensorTypeId,
                    MinValue = 0.5,
                    MaxValue = 2.0,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000548"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.PowerSensorTypeId,
                    MinValue = 25,
                    MaxValue = 45,
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
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000554"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.VoltageSensorTypeId,
                    MinValue = 11.5,
                    MaxValue = 12.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000555"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.CurrentSensorTypeId,
                    MinValue = 1.0,
                    MaxValue = 3.0,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000556"),
                    SpeciesId = SpeciesSeed.MudCrabId,
                    GrowthStageId = GrowthStageSeed.CrabGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.PowerSensorTypeId,
                    MinValue = 30,
                    MaxValue = 55,
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
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000562"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidParalarvaStageId,
                    SensorTypeId = SensorTypeSeed.VoltageSensorTypeId,
                    MinValue = 11.5,
                    MaxValue = 12.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000563"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidParalarvaStageId,
                    SensorTypeId = SensorTypeSeed.CurrentSensorTypeId,
                    MinValue = 0.5,
                    MaxValue = 1.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000564"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidParalarvaStageId,
                    SensorTypeId = SensorTypeSeed.PowerSensorTypeId,
                    MinValue = 20,
                    MaxValue = 35,
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
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000570"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.VoltageSensorTypeId,
                    MinValue = 11.5,
                    MaxValue = 12.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000571"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.CurrentSensorTypeId,
                    MinValue = 0.5,
                    MaxValue = 2.0,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000572"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidJuvenileStageId,
                    SensorTypeId = SensorTypeSeed.PowerSensorTypeId,
                    MinValue = 25,
                    MaxValue = 45,
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
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000578"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.VoltageSensorTypeId,
                    MinValue = 11.5,
                    MaxValue = 12.5,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000579"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.CurrentSensorTypeId,
                    MinValue = 1.0,
                    MaxValue = 3.0,
                },
                new SpeciesThreshold
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000580"),
                    SpeciesId = SpeciesSeed.ReefSquidId,
                    GrowthStageId = GrowthStageSeed.SquidGrowOutStageId,
                    SensorTypeId = SensorTypeSeed.PowerSensorTypeId,
                    MinValue = 30,
                    MaxValue = 55,
                },
            ];
    }
}
