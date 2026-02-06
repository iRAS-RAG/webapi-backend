using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class CameraSeed
    {
        // Timestamp nhất quán cho tất cả dữ liệu seed (UTC)
        private static readonly DateTime SeedTimestamp = new DateTime(
            2024,
            01,
            01,
            0,
            0,
            0,
            DateTimeKind.Utc
        );

        // Định nghĩa Guid cố định cho các camera seed
        // Mẫu: aaaaaaaa-0000-0000-0000-XXXXXXXXXXXX
        // Dải 701-799 dành cho Camera
        public static readonly Guid MainEntranceCameraId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000701"
        );

        public static readonly Guid Tank1CameraId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000702"
        );

        public static readonly Guid FeedingAreaCameraId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000703"
        );

        // Danh sách camera seed
        public static List<Camera> Cameras =>
            new()
            {
                new Camera
                {
                    Id = MainEntranceCameraId,
                    FarmId = FarmSeed.DefaultFarmId, // Tham chiếu đến Farm seed
                    Name = "Camera cổng chính",
                    Url = "rtsp://192.168.1.100:554/stream1",
                    IsDeleted = false,
                    CreatedAt = SeedTimestamp,
                },
                new Camera
                {
                    Id = Tank1CameraId,
                    FarmId = FarmSeed.DefaultFarmId,
                    Name = "Camera giám sát bể số 1",
                    Url = "rtsp://192.168.1.101:554/stream1",
                    IsDeleted = false,
                    CreatedAt = SeedTimestamp,
                },
                new Camera
                {
                    Id = FeedingAreaCameraId,
                    FarmId = FarmSeed.DefaultFarmId,
                    Name = "Camera khu vực cho ăn",
                    Url = "rtsp://192.168.1.102:554/stream1",
                    IsDeleted = false,
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
