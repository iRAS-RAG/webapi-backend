using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    public class SpeciesThresholdDto
    {
        public Guid Id { get; set; }
        public string SpeciesName { get; set; }
        public string GrowthStageName { get; set; }
        public float Low { get; set; }
        public float High { get; set; }
        public string Unit { get; set; }
    }

    public class CreateSpeciesThresholdDto
    {
        [Required]
        public Guid SpeciesId { get; set; }

        [Required]
        public Guid GrowthStageId { get; set; }

        [Required]
        public Guid SensorTypeId { get; set; }

        [Required]
        public float MinValue { get; set; }

        [Required]
        public float MaxValue { get; set; }
    }

    public class UpdateSpeciesThresholdDto
    {
        public float? MinValue { get; set; }

        public float? MaxValue { get; set; }
    }

    // List Request DTO
    public class SpeciesThresholdListRequest : BasePaginatedListRequest
    {
        public Guid? SpeciesId { get; set; }
    }
}
