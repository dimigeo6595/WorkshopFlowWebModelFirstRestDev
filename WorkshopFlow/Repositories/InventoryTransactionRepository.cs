using Microsoft.EntityFrameworkCore;
using WorkshopFlow.Data;
using WorkshopFlow.Models;

namespace WorkshopFlow.Repositories
{
    public class InventoryTransactionRepository : BaseRepository<InventoryTransaction>, IInventoryTransactionRepository
    {
        public InventoryTransactionRepository(WorkshopFlowContext context) : base(context)
        {
        }

        public override async Task<InventoryTransaction?> GetByIdAsync(int id) =>
            await _context.InventoryTransactions
                .Include(t => t.Item)
                .Include(t => t.CreatedBy)
                .Include(t => t.WorkOrder)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsByItemAsync(int itemId) =>
            await _context.InventoryTransactions
                .Include(t => t.Item)
                .Include(t => t.CreatedBy)
                .Include(t => t.WorkOrder)
                .Where(t => t.ItemId == itemId && !t.IsDeleted)
                .OrderByDescending(t => t.InsertedAt)
                .ToListAsync();

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsByWorkOrderAsync(int workOrderId) =>
            await _context.InventoryTransactions
                .Include(t => t.Item)
                .Include(t => t.CreatedBy)
                .Include(t => t.WorkOrder)
                .Where(t => t.WorkOrderId == workOrderId && !t.IsDeleted)
                .OrderByDescending(t => t.InsertedAt)
                .ToListAsync();
    }
}
