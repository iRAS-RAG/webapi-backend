using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class SpeciesSeed
    {
        public static readonly Guid TilapiaId =
            Guid.Parse("aaaaaaaa-0000-0000-0000-000000000101");

        public static List<Species> Species =>
            new()
            {
            new Species
            {
                Id = TilapiaId,
                Name = "Cá rô phi"
            }
            };
    }

}
