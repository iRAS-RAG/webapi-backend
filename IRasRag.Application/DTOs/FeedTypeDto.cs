using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    public class FeedTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float WeightPerUnit { get; set; }
        public float ProteinPercentage { get; set; }
        public string Manufacturer { get; set; }
    }

    public class CreateFeedTypeDto
    {
        [Required(ErrorMessage = "Tên loại thức ăn không được để trống.")]
        [MaxLength(255, ErrorMessage = "Tên loại thức ăn không được vượt quá 255 ký tự.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Trọng lượng mỗi đơn vị không được để trống.")]
        [Range(0.01, float.MaxValue, ErrorMessage = "Trọng lượng mỗi đơn vị phải lớn hơn 0.")]
        public float WeightPerUnit { get; set; }

        [Required(ErrorMessage = "Tỷ lệ protein không được để trống.")]
        [Range(0, 100, ErrorMessage = "Tỷ lệ protein phải từ 0 đến 100.")]
        public float ProteinPercentage { get; set; }

        [MaxLength(255, ErrorMessage = "Tên nhà sản xuất không được vượt quá 255 ký tự.")]
        public string Manufacturer { get; set; }
    }

    public class UpdateFeedTypeDto
    {
        [MaxLength(255, ErrorMessage = "Tên loại thức ăn không được vượt quá 255 ký tự.")]
        public string? Name { get; set; }

        [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? Description { get; set; }

        [Range(0.01, float.MaxValue, ErrorMessage = "Trọng lượng mỗi đơn vị phải lớn hơn 0.")]
        public float? WeightPerUnit { get; set; }

        [Range(0, 100, ErrorMessage = "Tỷ lệ protein phải từ 0 đến 100.")]
        public float? ProteinPercentage { get; set; }

        [MaxLength(255, ErrorMessage = "Tên nhà sản xuất không được vượt quá 255 ký tự.")]
        public string? Manufacturer { get; set; }
    }
}
