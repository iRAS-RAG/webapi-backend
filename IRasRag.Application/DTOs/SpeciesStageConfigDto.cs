using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    public class SpeciesStageConfigDto
    {
        public Guid Id { get; set; }
        public Guid SpeciesId { get; set; }
        public string SpeciesName { get; set; } = string.Empty;
        public Guid GrowthStageId { get; set; }
        public string GrowthStageName { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public List<Guid> FeedTypeIds { get; set; } = [];
        public List<string> FeedTypeNames { get; set; } = [];

        public double AmountPer100Fish { get; set; }

        public int FrequencyPerDay { get; set; }

        public double? MaxStockingDensity { get; set; }

        public int? ExpectedDurationDays { get; set; }

        // Growth estimation
        public double? ExpectedWeightKgPerFish { get; set; }
        public double? SurvivalRate { get; set; }
    }

    public class CreateSpeciesStageConfigDto
    {
        [Required]
        public Guid SpeciesId { get; set; }

        [Required]
        public Guid GrowthStageId { get; set; }

        public int? Sequence { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Phải chọn ít nhất một kiểu thức ăn.")]
        public List<Guid> FeedTypeIds { get; set; } = [];

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Lượng cho ăn phải lớn hơn 0 kg.")]
        public double AmountPer100Fish { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Tần suất cho ăn phải từ 1 lần mỗi ngày")]
        public int FrequencyPerDay { get; set; }

        [Range(
            0.01,
            double.MaxValue,
            ErrorMessage = "Mật độ thả nuôi tối đa phải lớn hơn 0 kg/m³."
        )]
        public double? MaxStockingDensity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thời gian dự kiến phải là số ngày hợp lệ.")]
        public int? ExpectedDurationDays { get; set; }

        [Range(
            0.0001,
            double.MaxValue,
            ErrorMessage = "Expected weight must be > 0 kg if provided."
        )]
        public double? ExpectedWeightKgPerFish { get; set; }

        [Range(0.0, 1.0, ErrorMessage = "SurvivalRate must be between 0 and 1.")]
        public double? SurvivalRate { get; set; }
    }

    public class UpdateSpeciesStageConfigDto
    {
        public List<Guid>? FeedTypeIds { get; set; }
        public int? Sequence { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Lượng cho ăn phải lớn hơn 0 kg.")]
        public double? AmountPer100Fish { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Tần suất cho ăn phải từ 1 lần mỗi ngày")]
        public int? FrequencyPerDay { get; set; }

        [Range(
            0.01,
            double.MaxValue,
            ErrorMessage = "Mật độ thả nuôi tối đa phải lớn hơn 0 kg/m³."
        )]
        public double? MaxStockingDensity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thời gian dự kiến phải là số ngày hợp lệ.")]
        public int? ExpectedDurationDays { get; set; }

        [Range(
            0.0001,
            double.MaxValue,
            ErrorMessage = "Expected weight must be > 0 kg if provided."
        )]
        public double? ExpectedWeightKgPerFish { get; set; }

        [Range(0.0, 1.0, ErrorMessage = "SurvivalRate must be between 0 and 1.")]
        public double? SurvivalRate { get; set; }
    }

    public class ReorderSpeciesStageConfigsDto
    {
        [Required]
        public Guid SpeciesId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Phải có ít nhất một cấu hình giai đoạn.")]
        public List<Guid> OrderedIds { get; set; } = [];
    }

    // List Request DTO
    public class SpeciesStageConfigListRequest : BasePaginatedListRequest { }
}
