namespace IRasRag.Application.DTOs
{
    public class AuditLogDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? fullName => $"{FirstName} {LastName}".Trim();
        public string Email { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
