using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    public class SpeciesThresholdDto
    {
        public Guid Id { get; set; }
        public Guid SpeciesId { get; set; }
        public string SpeciesName { get; set; }
        public Guid GrowthStageId { get; set; }
        public string GrowthStageName { get; set; }
        public Guid SensorTypeId { get; set; }
        public string SensorTypeName { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public string UnitOfMeasure { get; set; }
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
        public double MinValue { get; set; }

        [Required]
        public double MaxValue { get; set; }
    }

    public class UpdateSpeciesThresholdDto
    {
        public double? MinValue { get; set; }

        public double? MaxValue { get; set; }
    }

    // Lightweight threshold summary — only sensor-level fields (no species/stage noise)
    public class SensorThresholdSummaryDto
    {
        public Guid SensorTypeId { get; set; }
        public string SensorTypeName { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public string UnitOfMeasure { get; set; }
    }

    // List Request DTO
    public class SpeciesThresholdListRequest : BasePaginatedListRequest
    {
        public Guid? SpeciesId { get; set; }
    }
}
