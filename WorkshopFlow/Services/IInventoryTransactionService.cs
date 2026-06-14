using WorkshopFlow.DTO;

namespace WorkshopFlow.Services
{
    public interface IInventoryTransactionService
    {
        Task<IEnumerable<InventoryTransactionReadOnlyDTO>> GetTransactionsByItemAsync(int itemId);
        Task<IEnumerable<InventoryTransactionReadOnlyDTO>> GetTransactionsByWorkOrderAsync(int workOrderId);
        Task<InventoryTransactionReadOnlyDTO> InsertManualTransactionAsync(
            InventoryTransactionInsertDTO dto, int createdByUserId);
    }
}
