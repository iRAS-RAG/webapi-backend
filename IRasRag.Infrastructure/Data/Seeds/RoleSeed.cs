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

        public static readonly Guid AdminRoleId =
            Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");

        public static readonly Guid UserRoleId =
            Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002");

        public static List<Role> Roles =>
            new()
            {
            new Role
            {
                Id = AdminRoleId,
                Name = "Admin",
                CreatedAt = SeedTimestamp
            },
            new Role
            {
                Id = UserRoleId,
                Name = "User",
                CreatedAt = SeedTimestamp
            }
            };
    }

}
