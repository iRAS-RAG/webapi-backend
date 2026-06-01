namespace IRasRag.Application.Common.Constants
{
    public static class AuditLogActions
    {
        public const string Create = "CREATE";
        public const string Update = "UPDATE";
        public const string Delete = "DELETE";
        public const string Login = "LOGIN";
        public const string Logout = "LOGOUT";
        public const string RequestPasswordReset = "REQUEST_PASSWORD_RESET";
        public const string ResetPassword = "RESET_PASSWORD";
        public const string RefreshToken = "REFRESH_TOKEN";
        public const string HarvestBatch = "HARVEST_BATCH";
        public const string ToggleDevice = "TOGGLE_DEVICE";
    }
}