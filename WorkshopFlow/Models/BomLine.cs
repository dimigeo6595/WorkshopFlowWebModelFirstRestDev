namespace WorkshopFlow.Models
{
    public class BomLine : BaseEntity
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public string? Notes { get; set; }

        // FKs
        public int ProducedItemId { get; set; }
        public int ComponentItemId { get; set; }
        public int UnitOfMeasureId { get; set; }

        // Navigation properties
        public Item ProducedItem { get; set; } = null!;
        public Item ComponentItem { get; set; } = null!;
        public UnitOfMeasure UnitOfMeasure { get; set; } = null!;
    }
}
