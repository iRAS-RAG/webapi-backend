using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    public class GrowthStageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CreateGrowthStageDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }

    public class UpdateGrowthStageDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
