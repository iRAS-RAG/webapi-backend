using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Enums;

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

    // List Request DTO
    public class FishTankListRequest : BasePaginatedListRequest
    {
        public Guid? FarmId { get; set; }
    }

    // Quick status for a tank based on sensor warning flags
    public class TankStatusDto
    {
        public Guid TankId { get; set; }
        public string TankName { get; set; } = string.Empty;
        public TankStatus Status { get; set; }
        public int TotalSensors { get; set; }
        public int WarningSensors { get; set; }
    }

    // Latest sensor reading per sensor in a tank
    public class TankSensorLatestDataDto
    {
        public Guid SensorId { get; set; }
        public string SensorName { get; set; } = string.Empty;
        public Guid SensorTypeId { get; set; }
        public string SensorTypeName { get; set; } = string.Empty;
        public string MeasureType { get; set; } = string.Empty;
        public string UnitOfMeasure { get; set; } = string.Empty;
        public Guid MasterBoardId { get; set; }
        public string MasterBoardName { get; set; } = string.Empty;
        public double? LatestValue { get; set; }
        public bool? IsWarning { get; set; }
        public DateTime? RecordedAt { get; set; }
    }
}
