namespace WorkshopFlow.Models
{
    public class WorkOrderOperation : BaseEntity
    {
        public int Id { get; set; }
        public int Sequence { get; set; }
        public WorkOrderOperationStatus Status { get; set; } = WorkOrderOperationStatus.Pending;
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string? Notes { get; set; }

        // FKs
        public int WorkOrderId { get; set; }
        public int RoutingStepId { get; set; }
        public int? AssignedToUserId { get; set; }

        // Navigation properties
        public WorkOrder WorkOrder { get; set; } = null!;
        public RoutingStep RoutingStep { get; set; } = null!;
        public User? AssignedTo { get; set; }
    }
}
