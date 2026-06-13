namespace WorkshopFlow.Models
{
    public class Item : BaseEntity
    {
        public int Id { get; set; }
        public string ItemCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ItemType ItemType { get; set; }
        public decimal StockQuantity { get; set; } = 0;
        public decimal? WeightPerUoM { get; set; }
        public decimal? Weight { get; set; }

        // Computed — δεν αποθηκεύεται στη βάση
        public bool IsManufactured =>
            ItemType == ItemType.SemiFinished ||
            ItemType == ItemType.FinalProduct;

        // FKs
        public int UnitOfMeasureId { get; set; }
        public int? WeightUoMId { get; set; }

        // Navigation properties
        public UnitOfMeasure UnitOfMeasure { get; set; } = null!;
        public UnitOfMeasure? WeightUoM { get; set; }
    }
}