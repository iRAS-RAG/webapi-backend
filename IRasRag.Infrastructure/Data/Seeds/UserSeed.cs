using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class UserSeed
    {
        private static readonly DateTime SeedTimestamp = new DateTime(
            2024,
            01,
            01,
            0,
            0,
            0,
            DateTimeKind.Utc
        );

        // Password: "123456"
        public const string DefaultPasswordHash =
            "$2a$11$TjsTmXlpjjTVPajZiLxCV.XPuTPgCgphg7sfC9Fs/YwSA4M4IYqYu";

        public static readonly Guid AdminId =
            Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");

        public static readonly Guid UserId =
            Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002");

        public static List<User> Users =>
            new()
            {
            // ------------------------------
            // Admin user
            // ------------------------------
            new User
            {
                Id = AdminId,
                RoleId = RoleSeed.AdminRoleId,
                UserName = "admin",
                Email = "admin@example.com",
                PasswordHash = DefaultPasswordHash,
                IsVerified = true,
                CreatedAt = SeedTimestamp
            },

            // ------------------------------
            // Normal user
            // ------------------------------
            new User
            {
                Id = UserId,
                RoleId = RoleSeed.UserRoleId,
                UserName = "user",
                Email = "user@example.com",
                PasswordHash = DefaultPasswordHash,
                IsVerified = true,
                CreatedAt = SeedTimestamp
            }
            };
    }
}
