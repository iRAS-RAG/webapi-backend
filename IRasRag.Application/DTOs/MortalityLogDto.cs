using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class MortalityLogDto
    {
        public Guid Id { get; set; }
        public Guid BatchId { get; set; }
        public float Quantity { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    // Create DTO
    public class CreateMortalityLogDto
    {
        [Required(ErrorMessage = "Mã lô nuôi là bắt buộc")]
        public Guid BatchId { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(0.1, float.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public float Quantity { get; set; }

        [Required(ErrorMessage = "Ngày ghi nhận là bắt buộc")]
        public DateTime Date { get; set; }
    }

    // Update DTO
    public class UpdateMortalityLogDto
    {
        public Guid? BatchId { get; set; }

        [Range(0.1, float.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public float? Quantity { get; set; }

        public DateTime? Date { get; set; }
    }
}
