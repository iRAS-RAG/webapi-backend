namespace IRasRag.Application.DTOs
{
    public class AuditLogDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName => $"{LastName} {FirstName}".Trim();
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
