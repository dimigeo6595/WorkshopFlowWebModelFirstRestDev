using WorkshopFlow.Core;
using WorkshopFlow.Data;
using WorkshopFlow.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace WorkshopFlow.Repositories
{
    public class ItemRepository : BaseRepository<Item>, IItemRepository
    {
        public ItemRepository(WorkshopFlowContext context) : base(context)
        {
        }

        public override async Task<Item?> GetByIdAsync(int id) =>
            await _context.Items
                .Include(i => i.UnitOfMeasure)
                .Include(i => i.WeightUoM)
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);

        public async Task<Item?> GetItemByCodeAsync(string itemCode) =>
            await _context.Items
                .Include(i => i.UnitOfMeasure)
                .Include(i => i.WeightUoM)
                .FirstOrDefaultAsync(i => i.ItemCode == itemCode && !i.IsDeleted);

        public async Task<PaginatedResult<Item>> GetItemsAsync(int pageNumber, int pageSize,
            List<Expression<Func<Item, bool>>> predicates)
        {
            IQueryable<Item> query = _context.Items
                .Include(i => i.UnitOfMeasure)
                .Include(i => i.WeightUoM)
                .Where(i => !i.IsDeleted);

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }

            int totalRecords = await query.CountAsync();
            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(i => i.ItemCode)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Item>()
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
