namespace WorkshopFlow.DTO
{
    public record WorkOrderReadOnlyDTO
    {
        public int Id { get; set; }
        public string WorkOrderCode { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int ProducedItemId { get; set; }
        public string ProducedItemCode { get; set; } = null!;
        public string ProducedItemName { get; set; } = null!;
        public int Quantity { get; set; }
        public string UnitOfMeasureSymbol { get; set; } = null!;
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string CreatedByUsername { get; set; } = null!;
        public int TotalOperations { get; set; }
        public int CompletedOperations { get; set; }
        public string? Notes { get; set; }
    }
}
