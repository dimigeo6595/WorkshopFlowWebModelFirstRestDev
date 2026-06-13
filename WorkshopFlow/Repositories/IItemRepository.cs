using WorkshopFlow.Core;
using WorkshopFlow.Models;
using System.Linq.Expressions;

namespace WorkshopFlow.Repositories
{
    public interface IItemRepository : IBaseRepository<Item>
    {
        Task<Item?> GetItemByCodeAsync(string itemCode);
        Task<PaginatedResult<Item>> GetItemsAsync(int pageNumber, int pageSize,
            List<Expression<Func<Item, bool>>> predicates);
    }
}
