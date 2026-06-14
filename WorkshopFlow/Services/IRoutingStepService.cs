using WorkshopFlow.DTO;

namespace WorkshopFlow.Services
{
    public interface IRoutingStepService
    {
        Task<IEnumerable<RoutingStepReadOnlyDTO>> GetRoutingByItemIdAsync(int producedItemId);
        Task<RoutingStepReadOnlyDTO> InsertRoutingStepAsync(int producedItemId, RoutingStepInsertDTO dto);
        Task<RoutingStepReadOnlyDTO> UpdateRoutingStepAsync(int producedItemId, int stepId, RoutingStepUpdateDTO dto);
        Task DeleteRoutingStepAsync(int producedItemId, int stepId);
    }
}
