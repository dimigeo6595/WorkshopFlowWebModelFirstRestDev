namespace WorkshopFlow.Models
{
    public class UnitOfMeasure : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Symbol { get; set; } = null!;

        // Navigation properties
        public ICollection<Item> Items { get; set; } = new HashSet<Item>();
        public ICollection<Item> WeightItems { get; set; } = new HashSet<Item>();
    }
}