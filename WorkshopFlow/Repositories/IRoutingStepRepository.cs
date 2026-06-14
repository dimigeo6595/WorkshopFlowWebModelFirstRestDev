using WorkshopFlow.Models;

namespace WorkshopFlow.Repositories
{
    public interface IRoutingStepRepository : IBaseRepository<RoutingStep>
    {
        Task<IEnumerable<RoutingStep>> GetRoutingByProducedItemIdAsync(int producedItemId);
        Task<RoutingStep?> GetRoutingStepAsync(int producedItemId, int stepId);
        Task<bool> SequenceExistsAsync(int producedItemId, int sequence, int? excludeStepId = null);
    }
}
