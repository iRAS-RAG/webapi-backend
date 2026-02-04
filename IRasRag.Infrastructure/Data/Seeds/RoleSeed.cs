using IRasRag.Domain.Entities;

namespace IRasRag.Infrastructure.Data.Seeds
{
    public static class RoleSeed
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

        public static readonly Guid AdminRoleId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000001"
        );

        public static readonly Guid SupervisorRoleId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000002"
        );

        public static readonly Guid OperatorRoleId = Guid.Parse(
            "aaaaaaaa-0000-0000-0000-000000000003"
        );

        public static List<Role> Roles =>
            new()
            {
                new Role
                {
                    Id = AdminRoleId,
                    Name = "Admin",
                    CreatedAt = SeedTimestamp,
                },
                new Role
                {
                    Id = SupervisorRoleId,
                    Name = "Supervisor",
                    CreatedAt = SeedTimestamp,
                },
                new Role
                {
                    Id = OperatorRoleId,
                    Name = "Operator",
                    CreatedAt = SeedTimestamp,
                },
            };
    }
}
