using WorkshopFlow.Models;

namespace WorkshopFlow.Repositories
{
    public interface IBomLineRepository : IBaseRepository<BomLine>
    {
        Task<IEnumerable<BomLine>> GetBomByProducedItemIdAsync(int producedItemId);
        Task<BomLine?> GetBomLineAsync(int producedItemId, int bomLineId);
        Task<bool> ComponentExistsInBomAsync(int producedItemId, int componentItemId);
    }
}
