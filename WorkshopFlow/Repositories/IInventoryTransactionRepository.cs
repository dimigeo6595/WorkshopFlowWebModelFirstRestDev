using WorkshopFlow.Models;

namespace WorkshopFlow.Repositories
{
    public interface IInventoryTransactionRepository : IBaseRepository<InventoryTransaction>
    {
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByItemAsync(int itemId);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByWorkOrderAsync(int workOrderId);
    }
}
