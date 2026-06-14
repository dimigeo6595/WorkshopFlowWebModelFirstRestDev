using System.ComponentModel.DataAnnotations;

namespace WorkshopFlow.DTO
{
    public record WorkOrderInsertDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        public int? ProducedItemId { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int? Quantity { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public DateTime? PlannedStartDate { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public DateTime? PlannedEndDate { get; set; }

        [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}

