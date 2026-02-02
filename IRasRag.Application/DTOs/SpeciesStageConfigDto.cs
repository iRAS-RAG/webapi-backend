using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    public class SpeciesStageConfigDto
    {
        public Guid Id { get; set; }
        public string SpeciesName { get; set; }

        public string GrowthStageName { get; set; }

        public string FeedTypeName { get; set; }

        public float AmountPer100Fish { get; set; }

        public int FrequencyPerDay { get; set; }

        public float? MaxStockingDensity { get; set; }

        public int? ExpectedDurationDays { get; set; }
    }

    public class CreateSpeciesStageConfigDto
    {
        [Required]
        public Guid SpeciesId { get; set; }

        [Required]
        public Guid GrowthStageId { get; set; }

        [Required]
        public Guid FeedTypeId { get; set; }

        [Required]
        [Range(0.01, float.MaxValue, ErrorMessage = "Lượng cho ăn phải lớn hơn 0 kg.")]
        public float AmountPer100Fish { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Tần suất cho ăn phải từ 1 lần mỗi ngày")]
        public int FrequencyPerDay { get; set; }

        [Range(0.01, float.MaxValue, ErrorMessage = "Mật độ thả nuôi tối đa phải lớn hơn 0 kg/m³.")]
        public float? MaxStockingDensity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thời gian dự kiến phải là số ngày hợp lệ.")]
        public int? ExpectedDurationDays { get; set; }
    }

    public class UpdateSpeciesStageConfigDto
    {
        public Guid? FeedTypeId { get; set; }

        [Range(0.01, float.MaxValue, ErrorMessage = "Lượng cho ăn phải lớn hơn 0 kg.")]
        public float? AmountPer100Fish { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Tần suất cho ăn phải từ 1 lần mỗi ngày")]
        public int? FrequencyPerDay { get; set; }

        [Range(0.01, float.MaxValue, ErrorMessage = "Mật độ thả nuôi tối đa phải lớn hơn 0 kg/m³.")]
        public float? MaxStockingDensity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thời gian dự kiến phải là số ngày hợp lệ.")]
        public int? ExpectedDurationDays { get; set; }
    }
}
