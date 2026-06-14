using System.ComponentModel.DataAnnotations;

namespace WorkshopFlow.DTO
{
    public record RoutingStepUpdateDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Sequence must be greater than 0.")]
        public int? Sequence { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "OperationName must be between 2 and 100 characters.")]
        public string? OperationName { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "EstimatedMinutes must be greater than 0.")]
        public int? EstimatedMinutes { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public int? WorkstationId { get; set; }

        public int? MachineId { get; set; }

        [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}
