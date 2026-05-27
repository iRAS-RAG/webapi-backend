using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    public class AuditLogQueryRequest : BasePaginatedListRequest
    {
        public Guid? UserId { get; set; }

        [MaxLength(50)]
        public string? Action { get; set; }

        [MaxLength(100)]
        public string? EntityType { get; set; }

        [MaxLength(100)]
        public string? EntityId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}