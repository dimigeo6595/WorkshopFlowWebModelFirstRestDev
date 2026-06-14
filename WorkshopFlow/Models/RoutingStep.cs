namespace WorkshopFlow.Models
{
    public class RoutingStep : BaseEntity
    {
        public int Id { get; set; }
        public int Sequence { get; set; }
        public string OperationName { get; set; } = null!;
        public int EstimatedMinutes { get; set; }
        public string? Notes { get; set; }

        // FKs
        public int ProducedItemId { get; set; }
        public int WorkstationId { get; set; }
        public int? MachineId { get; set; }

        // Navigation properties
        public Item ProducedItem { get; set; } = null!;
        public Workstation Workstation { get; set; } = null!;
        public Machine? Machine { get; set; }
        public ICollection<WorkOrderOperation> WorkOrderOperations { get; set; } = new HashSet<WorkOrderOperation>();

    }
}
