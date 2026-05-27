using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class GrowthStageSeed
    {
        public static readonly Guid FryStageId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000401");

        public static readonly Guid JuvenileStageId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000402"
        );

        public static List<GrowthStage> GrowthStages =>
            [
                new GrowthStage
                {
                    Id = FryStageId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    Name = "Cá bột",
                    Description =
                        "Giai đoạn từ khi nở đến khi cá phát triển đủ lớn để chuyển sang giai đoạn cá giống.",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                new GrowthStage
                {
                    Id = JuvenileStageId,
                    SpeciesId = SpeciesSeed.TilapiaId,
                    Name = "Cá giống",
                    Description = "Giai đoạn từ cá bột đến khi cá đạt kích thước thương phẩm.",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
            ];
    }
}
