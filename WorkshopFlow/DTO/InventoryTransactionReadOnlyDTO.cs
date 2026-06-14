namespace WorkshopFlow.DTO
{
    public record InventoryTransactionReadOnlyDTO
    {
        public int Id { get; set; }
        public string TransactionType { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string ItemCode { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public string? WorkOrderCode { get; set; }  // nullable — Purchase/Adjustment δεν έχουν WO
        public string CreatedByUsername { get; set; } = null!;
        public DateTime InsertedAt { get; set; }
        public string? Notes { get; set; }
    }
}
