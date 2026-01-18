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

        public static List<User> Users =>
            new()
            {
            // ------------------------------
            // Admin user
            // ------------------------------
            new User
            {
                Id = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000001"),
                RoleId = RoleSeed.AdminRoleId,
                UserName = "admin",
                PasswordHash = DefaultPasswordHash,
                IsVerified = true,
                CreatedAt = SeedTimestamp
            },

            // ------------------------------
            // Normal user
            // ------------------------------
            new User
            {
                Id = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000002"),
                RoleId = RoleSeed.UserRoleId,
                UserName = "user",
                PasswordHash = DefaultPasswordHash,
                IsVerified = true,
                CreatedAt = SeedTimestamp
            }
            };
    }
}
