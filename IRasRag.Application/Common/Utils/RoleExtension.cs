using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Common.Utils
{
    public static class RoleExtensions
    {
        public static SystemRole ToSystemRole(this Role role)
        {
            return role.Name.ToLower() switch
            {
                "admin" => SystemRole.Admin,
                "operator" => SystemRole.Operator,
                "supervisor" => SystemRole.Supervisor,
                _ => throw new InvalidOperationException("Unknown role"),
            };
        }

        public static string ToRoleName(this SystemRole systemRole)
        {
            return systemRole switch
            {
                SystemRole.Admin => "Quản trị viên",
                SystemRole.Operator => "Kỹ thuật viên",
                SystemRole.Supervisor => "Quản lý",
                _ => throw new InvalidOperationException("Unknown system role"),
            };
        }
    }
}
