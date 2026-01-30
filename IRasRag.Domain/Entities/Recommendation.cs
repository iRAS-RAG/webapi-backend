using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class Recommendation : BaseEntity
    {
        [Required]
        public Guid AlertId { get; set; }

        [Required]
        public Guid DocumentId { get; set; }

        [Required]
        public string SuggestionText { get; set; }

        // Navigation properties
        public Alert Alert { get; set; }
        public Document Document { get; set; }
    }
}
