using WorkshopFlow.Models.Enums;

namespace WorkshopFlow.Models
{
    public class InventoryTransaction : BaseEntity
    {
        public int Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public string? Notes { get; set; }

        // FKs
        public int ItemId { get; set; }
        public int? WorkOrderId { get; set; }
        public int CreatedByUserId { get; set; }

        // Navigation properties
        public Item Item { get; set; } = null!;
        public WorkOrder? WorkOrder { get; set; }
        public User CreatedBy { get; set; } = null!;
    }
}
