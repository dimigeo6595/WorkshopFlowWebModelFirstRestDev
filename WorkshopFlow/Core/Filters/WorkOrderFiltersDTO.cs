using WorkshopFlow.Models;

namespace WorkshopFlow.Core.Filters
{
    public class WorkOrderFiltersDTO
    {
        public WorkOrderStatus? Status { get; set; }
        public int? ProducedItemId { get; set; }
    }
}
