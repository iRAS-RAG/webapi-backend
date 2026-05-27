using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class FishTankSeed
    {
        public static readonly Guid TankAId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000301");

        private static readonly DateTime SeedTimestamp = new(
            2024,
            01,
            01,
            0,
            0,
            0,
            DateTimeKind.Utc
        );

        public static List<FishTank> FishTanks =>
            [
                new FishTank
                {
                    Id = TankAId,
                    FarmId = FarmSeed.DefaultFarmId,
                    Name = "Bể nuôi số 1",
                    Height = 1.2, // meters
                    Radius = 1.8, // meters
                    TopicCode = "tank/001",
                    CameraUrl = "rtsp://192.168.1.101:554/stream1",
                    CreatedAt = SeedTimestamp,
                    ModifiedAt = SeedTimestamp,
                },
            ];
    }
}
