namespace WorkshopFlow.DTO
{
    public record ItemReadOnlyDTO
    {
        public int Id { get; set; }
        public string ItemCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }      // optional
        public string ItemType { get; set; } = null!;
        public bool IsManufactured { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal? WeightPerUoM { get; set; }    // optional
        public decimal? Weight { get; set; }           // optional
        public string UnitOfMeasureSymbol { get; set; } = null!;
        public string? WeightUoMSymbol { get; set; }  // optional
    }
}