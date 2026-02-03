using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class FarmSeed
    {
        public static readonly Guid DefaultFarmId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000001"
        );

        private static readonly DateTime SeedTimestamp = new DateTime(
            2024,
            01,
            01,
            0,
            0,
            0,
            DateTimeKind.Utc
        );

        public static List<Farm> Farms =>
            new()
            {
                new Farm
                {
                    Id = DefaultFarmId,
                    Name = "Trang trại RAS mẫu",
                    Address = "Đường 123, Tp.HCM",
                    PhoneNumber = "+84-123-456-789",
                    Email = "contact@aquabluefarm.vn",
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
