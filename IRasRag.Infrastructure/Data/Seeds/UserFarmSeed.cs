using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class UserFarmSeed
    {
        public static List<UserFarm> UserFarms =>
            new()
            {
                new UserFarm
                {
                    Id = Guid.Parse("44444444-0001-0001-0001-000000000001"),
                    UserId = UserSeed.SupervisorId,
                    FarmId = FarmSeed.DefaultFarmId,
                },
            };
    }
}
