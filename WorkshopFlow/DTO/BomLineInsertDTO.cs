using System.ComponentModel.DataAnnotations;

namespace WorkshopFlow.DTO
{
    public record BomLineInsertDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        public int? ComponentItemId { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public decimal? Quantity { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public int? UnitOfMeasureId { get; set; }

        [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}
