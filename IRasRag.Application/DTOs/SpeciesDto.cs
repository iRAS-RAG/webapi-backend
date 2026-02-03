using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    public class SpeciesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class CreateSpeciesDto
    {
        [Required(ErrorMessage = "Tên loài không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên loài không được vượt quá 100 ký tự.")]
        public string Name { get; set; }
    }

    public class UpdateSpeciesDto
    {
        [MaxLength(100, ErrorMessage = "Tên loài không được vượt quá 100 ký tự.")]
        public string? Name { get; set; }
    }
}
