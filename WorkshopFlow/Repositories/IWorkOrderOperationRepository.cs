using WorkshopFlow.Models;

namespace WorkshopFlow.Repositories
{
    public interface IWorkOrderOperationRepository : IBaseRepository<WorkOrderOperation>
    {
        Task<WorkOrderOperation?> GetOperationAsync(int workOrderId, int operationId);
        Task<IEnumerable<WorkOrderOperation>> GetOperationsByWorkOrderAsync(int workOrderId);
        Task<WorkOrderOperation?> GetPreviousOperationAsync(int workOrderId, int sequence);
    }
}
