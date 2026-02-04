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

        public static readonly Guid AdminId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");

        public static readonly Guid SupervisorId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000002"
        );

        public static readonly Guid OperatorId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003");

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
                    Email = "admin@example.com",
                    PasswordHash = DefaultPasswordHash,
                    FirstName = "Văn A",
                    LastName = "Nguyễn",
                    CreatedAt = SeedTimestamp,
                },
                // ------------------------------
                // Supervisor user
                // ------------------------------
                new User
                {
                    Id = SupervisorId,
                    RoleId = RoleSeed.SupervisorRoleId,
                    Email = "supervisor@example.com",
                    PasswordHash = DefaultPasswordHash,
                    FirstName = "Thị B",
                    LastName = "Trần",
                    CreatedAt = SeedTimestamp,
                },
                // ------------------------------
                // Operator user
                // ------------------------------
                new User
                {
                    Id = OperatorId,
                    RoleId = RoleSeed.OperatorRoleId,
                    Email = "operator@example.com",
                    PasswordHash = DefaultPasswordHash,
                    FirstName = "Văn C",
                    LastName = "Lê",
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
