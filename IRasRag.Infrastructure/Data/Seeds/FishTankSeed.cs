using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class FishTankSeed
    {
        public static readonly Guid TankAId =
            Guid.Parse("aaaaaaaa-0000-0000-0000-000000000301");

        public static List<FishTank> FishTanks =>
            new()
            {
            new FishTank
            {
                Id = TankAId,
                FarmId = FarmSeed.DefaultFarmId,
                Name = "Bể nuôi số 1"
            }
            };
    }

}
