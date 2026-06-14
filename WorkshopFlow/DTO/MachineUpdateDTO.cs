using System.ComponentModel.DataAnnotations;

namespace WorkshopFlow.DTO
{
    public record MachineUpdateDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public int? WorkstationId { get; set; }

        [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}
