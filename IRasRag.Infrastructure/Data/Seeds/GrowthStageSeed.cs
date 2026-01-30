using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class GrowthStageSeed
    {
        public static readonly Guid FryStageId =
            Guid.Parse("aaaaaaaa-0000-0000-0000-000000000401");

        public static readonly Guid JuvenileStageId =
            Guid.Parse("aaaaaaaa-0000-0000-0000-000000000402");

        public static List<GrowthStage> GrowthStages =>
            new()
            {
            new GrowthStage
            {
                Id = FryStageId,
                Name = "Cá bột"
            },
            new GrowthStage
            {
                Id = JuvenileStageId,
                Name = "Cá giống"
            }
            };
    }

}
