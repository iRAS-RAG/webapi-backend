using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    public class FishTankDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Height { get; set; }
        public float Radius { get; set; }
        public Guid FarmId { get; set; }
        public string FarmName { get; set; }
        public string TopicCode { get; set; }
        public string CameraUrl { get; set; }
    }

    public class CreateFishTankDto
    {
        [Required(ErrorMessage = "Tên bể cá không được để trống.")]
        [MaxLength(255, ErrorMessage = "Tên bể cá không được vượt quá 255 ký tự.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Chiều cao không được để trống.")]
        [Range(0.01, float.MaxValue, ErrorMessage = "Chiều cao phải lớn hơn 0.")]
        public float Height { get; set; }

        [Required(ErrorMessage = "Bán kính không được để trống.")]
        [Range(0.01, float.MaxValue, ErrorMessage = "Bán kính phải lớn hơn 0.")]
        public float Radius { get; set; }

        [Required(ErrorMessage = "ID trang trại không được để trống.")]
        public Guid FarmId { get; set; }

        [MaxLength(50, ErrorMessage = "Mã chủ đề không được vượt quá 50 ký tự.")]
        public string TopicCode { get; set; }

        [Required(ErrorMessage = "URL camera không được để trống.")]
        [MaxLength(255, ErrorMessage = "URL camera không được vượt quá 255 ký tự.")]
        [Url(ErrorMessage = "URL camera không hợp lệ.")]
        public string CameraUrl { get; set; }
    }

    public class UpdateFishTankDto
    {
        [MaxLength(255, ErrorMessage = "Tên bể cá không được vượt quá 255 ký tự.")]
        public string? Name { get; set; }

        [Range(0.01, float.MaxValue, ErrorMessage = "Chiều cao phải lớn hơn 0.")]
        public float? Height { get; set; }

        [Range(0.01, float.MaxValue, ErrorMessage = "Bán kính phải lớn hơn 0.")]
        public float? Radius { get; set; }

        public Guid? FarmId { get; set; }

        [MaxLength(50, ErrorMessage = "Mã chủ đề không được vượt quá 50 ký tự.")]
        public string? TopicCode { get; set; }

        [MaxLength(255, ErrorMessage = "URL camera không được vượt quá 255 ký tự.")]
        [Url(ErrorMessage = "URL camera không hợp lệ.")]
        public string? CameraUrl { get; set; }
    }
}
