using WorkshopFlow.Models;

namespace WorkshopFlow.Core.Filters
{
    public class ItemFiltersDTO
    {
        public string? Name { get; set; }
        public ItemType? ItemType { get; set; }
    }
}