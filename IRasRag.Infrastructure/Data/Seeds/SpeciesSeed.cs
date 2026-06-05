using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class SpeciesSeed
    {
        public static readonly Guid TilapiaId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000101");

        public static readonly Guid MudCrabId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000102");

        public static readonly Guid ReefSquidId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000103"
        );

        public static List<Species> Species =>
            [
                new Species
                {
                    Id = TilapiaId,
                    Name = "Cá rô phi",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                new Species
                {
                    Id = MudCrabId,
                    Name = "Cua biển",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
                new Species
                {
                    Id = ReefSquidId,
                    Name = "Mực lá",
                    CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                },
            ];
    }
}
