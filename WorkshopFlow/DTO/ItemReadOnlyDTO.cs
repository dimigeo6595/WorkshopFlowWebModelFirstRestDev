namespace WorkshopFlow.DTO
{
    public record ItemReadOnlyDTO
    {
        public int Id { get; set; }
        public string? ItemCode { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ItemType { get; set; }
        public bool IsManufactured { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal? WeightPerUoM { get; set; }
        public decimal? Weight { get; set; }
        public string? UnitOfMeasureSymbol { get; set; }
        public string? WeightUoMSymbol { get; set; }
    }
}
