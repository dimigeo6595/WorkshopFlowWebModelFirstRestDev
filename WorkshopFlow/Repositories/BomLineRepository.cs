using Microsoft.EntityFrameworkCore;
using WorkshopFlow.Data;
using WorkshopFlow.Models;

namespace WorkshopFlow.Repositories
{
    public class BomLineRepository : BaseRepository<BomLine>, IBomLineRepository
    {
        public BomLineRepository(WorkshopFlowContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BomLine>> GetBomByProducedItemIdAsync(int producedItemId) =>
            await _context.BomLines
                .Include(b => b.ComponentItem)
                    .ThenInclude(i => i.UnitOfMeasure)
                .Include(b => b.UnitOfMeasure)
                .Where(b => b.ProducedItemId == producedItemId && !b.IsDeleted)
                .OrderBy(b => b.ComponentItem.ItemCode)
                .ToListAsync();

        public async Task<BomLine?> GetBomLineAsync(int producedItemId, int bomLineId) =>
            await _context.BomLines
                .Include(b => b.ComponentItem)
                    .ThenInclude(i => i.UnitOfMeasure)
                .Include(b => b.UnitOfMeasure)
                .FirstOrDefaultAsync(b =>
                    b.Id == bomLineId &&
                    b.ProducedItemId == producedItemId &&
                    !b.IsDeleted);

        public async Task<bool> ComponentExistsInBomAsync(int producedItemId, int componentItemId) =>
            await _context.BomLines
                .AnyAsync(b =>
                    b.ProducedItemId == producedItemId &&
                    b.ComponentItemId == componentItemId &&
                    !b.IsDeleted);
    }
}
