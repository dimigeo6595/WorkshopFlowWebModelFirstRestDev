using System.ComponentModel.DataAnnotations;

namespace WorkshopFlow.DTO
{
    public record WorkOrderOperationAssignDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        public int? AssignedToUserId { get; set; }
    }
}
