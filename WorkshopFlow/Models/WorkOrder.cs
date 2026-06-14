namespace WorkshopFlow.Models
{
    public class WorkOrder : BaseEntity
    {
        public int Id { get; set; }
        public string WorkOrderCode { get; set; } = null!;
        public int Quantity { get; set; }
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Draft;
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string? Notes { get; set; }

        // FKs
        public int ProducedItemId { get; set; }
        public int CreatedByUserId { get; set; }

        // Navigation properties
        public Item ProducedItem { get; set; } = null!;
        public User CreatedBy { get; set; } = null!;
        public ICollection<WorkOrderOperation> Operations { get; set; } = new HashSet<WorkOrderOperation>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new HashSet<InventoryTransaction>();
    }
}
