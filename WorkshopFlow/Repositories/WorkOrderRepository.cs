using Microsoft.EntityFrameworkCore;
using WorkshopFlow.Core;
using WorkshopFlow.Data;
using WorkshopFlow.Models;
using System.Linq.Expressions;

namespace WorkshopFlow.Repositories
{
    public class WorkOrderRepository : BaseRepository<WorkOrder>, IWorkOrderRepository
    {
        public WorkOrderRepository(WorkshopFlowContext context) : base(context)
        {
        }

        public override async Task<WorkOrder?> GetByIdAsync(int id) =>
            await _context.WorkOrders
                .Include(w => w.ProducedItem)
                    .ThenInclude(i => i.UnitOfMeasure)
                .Include(w => w.CreatedBy)
                .Where(w => w.Id == id && !w.IsDeleted)
                .FirstOrDefaultAsync();

        public async Task<WorkOrder?> GetWorkOrderByCodeAsync(string workOrderCode) =>
            await _context.WorkOrders
                .Include(w => w.ProducedItem)
                .Include(w => w.CreatedBy)
                .FirstOrDefaultAsync(w => w.WorkOrderCode == workOrderCode && !w.IsDeleted);

        public async Task<WorkOrder?> GetWorkOrderWithDetailsAsync(int id) =>
            await _context.WorkOrders
                .Include(w => w.ProducedItem)
                    .ThenInclude(i => i.UnitOfMeasure)
                .Include(w => w.CreatedBy)
                .Include(w => w.Operations.Where(o => !o.IsDeleted))
                    .ThenInclude(o => o.RoutingStep)
                        .ThenInclude(r => r.Workstation)
                .Include(w => w.Operations.Where(o => !o.IsDeleted))
                    .ThenInclude(o => o.RoutingStep)
                        .ThenInclude(r => r.Machine)
                .Include(w => w.Operations.Where(o => !o.IsDeleted))
                    .ThenInclude(o => o.AssignedTo)
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);

        public async Task<PaginatedResult<WorkOrder>> GetWorkOrdersAsync(int pageNumber, int pageSize,
            List<Expression<Func<WorkOrder, bool>>> predicates)
        {
            IQueryable<WorkOrder> query = _context.WorkOrders
                .Include(w => w.ProducedItem)
                .Include(w => w.CreatedBy)
                .Where(w => !w.IsDeleted);

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
                .OrderByDescending(w => w.InsertedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<WorkOrder>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
