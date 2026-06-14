using System.ComponentModel.DataAnnotations;

namespace WorkshopFlow.DTO
{
    public record MachineInsertDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Code must be between 2 and 50 characters.")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public int? WorkstationId { get; set; }

        [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}
