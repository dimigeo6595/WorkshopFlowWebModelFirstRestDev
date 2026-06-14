using WorkshopFlow.Core;
using WorkshopFlow.Core.Filters;
using WorkshopFlow.DTO;

namespace WorkshopFlow.Services
{
    public interface IWorkOrderService
    {
        Task<PaginatedResult<WorkOrderReadOnlyDTO>> GetPaginatedWorkOrdersAsync(
            int pageNumber, int pageSize, WorkOrderFiltersDTO filters);
        Task<WorkOrderReadOnlyDTO> GetWorkOrderByIdAsync(int id);
        Task<WorkOrderReadOnlyDTO> InsertWorkOrderAsync(WorkOrderInsertDTO dto, int createdByUserId);
        Task<WorkOrderReadOnlyDTO> UpdateWorkOrderAsync(int id, WorkOrderUpdateDTO dto);
        Task DeleteWorkOrderAsync(int id);

        // Status transitions
        Task<WorkOrderReadOnlyDTO> ReleaseWorkOrderAsync(int id);
        Task CancelWorkOrderAsync(int id);

        // Operations
        Task<IEnumerable<WorkOrderOperationReadOnlyDTO>> GetOperationsAsync(int workOrderId);
        Task<WorkOrderOperationReadOnlyDTO> AssignOperationAsync(int workOrderId, int operationId, WorkOrderOperationAssignDTO dto);
        Task<WorkOrderOperationReadOnlyDTO> StartOperationAsync(int workOrderId, int operationId);
        Task<WorkOrderOperationReadOnlyDTO> CompleteOperationAsync(int workOrderId, int operationId);
    }
}
