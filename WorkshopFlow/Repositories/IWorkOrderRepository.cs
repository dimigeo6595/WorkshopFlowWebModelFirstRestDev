using WorkshopFlow.Core;
using WorkshopFlow.Models;
using System.Linq.Expressions;

namespace WorkshopFlow.Repositories
{
    public interface IWorkOrderRepository : IBaseRepository<WorkOrder>
    {
        Task<WorkOrder?> GetWorkOrderByCodeAsync(string workOrderCode);
        Task<WorkOrder?> GetWorkOrderWithDetailsAsync(int id);
        Task<PaginatedResult<WorkOrder>> GetWorkOrdersAsync(int pageNumber, int pageSize,
            List<Expression<Func<WorkOrder, bool>>> predicates);
    }
}
