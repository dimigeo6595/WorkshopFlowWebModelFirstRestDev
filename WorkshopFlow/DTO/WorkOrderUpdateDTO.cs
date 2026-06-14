using System.ComponentModel.DataAnnotations;

namespace WorkshopFlow.DTO
{
    public record WorkOrderUpdateDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        public DateTime? PlannedStartDate { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public DateTime? PlannedEndDate { get; set; }

        [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}
