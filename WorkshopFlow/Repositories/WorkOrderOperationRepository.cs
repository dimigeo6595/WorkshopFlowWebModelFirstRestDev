using Microsoft.EntityFrameworkCore;
using WorkshopFlow.Data;
using WorkshopFlow.Models;

namespace WorkshopFlow.Repositories
{
    public class WorkOrderOperationRepository : BaseRepository<WorkOrderOperation>, IWorkOrderOperationRepository
    {
        public WorkOrderOperationRepository(WorkshopFlowContext context) : base(context)
        {
        }

        public override async Task<WorkOrderOperation?> GetByIdAsync(int id) =>
            await _context.WorkOrderOperations
                .Include(o => o.RoutingStep)
                    .ThenInclude(r => r.Workstation)
                .Include(o => o.RoutingStep)
                    .ThenInclude(r => r.Machine)
                .Include(o => o.AssignedTo)
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);

        public async Task<WorkOrderOperation?> GetOperationAsync(int workOrderId, int operationId) =>
            await _context.WorkOrderOperations
                .Include(o => o.RoutingStep)
                    .ThenInclude(r => r.Workstation)
                .Include(o => o.RoutingStep)
                    .ThenInclude(r => r.Machine)
                .Include(o => o.AssignedTo)
                .FirstOrDefaultAsync(o =>
                    o.Id == operationId &&
                    o.WorkOrderId == workOrderId &&
                    !o.IsDeleted);

        public async Task<IEnumerable<WorkOrderOperation>> GetOperationsByWorkOrderAsync(int workOrderId) =>
            await _context.WorkOrderOperations
                .Include(o => o.RoutingStep)
                    .ThenInclude(r => r.Workstation)
                .Include(o => o.RoutingStep)
                    .ThenInclude(r => r.Machine)
                .Include(o => o.AssignedTo)
                .Where(o => o.WorkOrderId == workOrderId && !o.IsDeleted)
                .OrderBy(o => o.Sequence)
                .ToListAsync();

        public async Task<WorkOrderOperation?> GetPreviousOperationAsync(int workOrderId, int sequence) =>
            await _context.WorkOrderOperations
                .Where(o => o.WorkOrderId == workOrderId &&
                            o.Sequence < sequence &&
                            !o.IsDeleted)
                .OrderByDescending(o => o.Sequence)
                .FirstOrDefaultAsync();
    }
}

