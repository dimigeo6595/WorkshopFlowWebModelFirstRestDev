namespace WorkshopFlow.DTO
{
    public record BomLineReadOnlyDTO
    {
        public int Id { get; set; }
        public int ComponentItemId { get; set; }
        public string ComponentItemCode { get; set; } = null!;
        public string ComponentItemName { get; set; } = null!;
        public decimal Quantity { get; set; }
        public string UnitOfMeasureSymbol { get; set; } = null!;
        public string? Notes { get; set; }  // μόνο αυτό μένει nullable γιατί είναι optional
    }
}
