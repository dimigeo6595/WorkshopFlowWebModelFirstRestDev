using Microsoft.EntityFrameworkCore;
using WorkshopFlow.Data;
using WorkshopFlow.Models;

namespace WorkshopFlow.Repositories
{
    public class RoutingStepRepository : BaseRepository<RoutingStep>, IRoutingStepRepository
    {
        public RoutingStepRepository(WorkshopFlowContext context) : base(context)
        {
        }

        public override async Task<RoutingStep?> GetByIdAsync(int id) =>
            await _context.RoutingSteps
                .Include(r => r.Workstation)
                .Include(r => r.Machine)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        public async Task<IEnumerable<RoutingStep>> GetRoutingByProducedItemIdAsync(int producedItemId) =>
            await _context.RoutingSteps
                .Include(r => r.Workstation)
                .Include(r => r.Machine)
                .Where(r => r.ProducedItemId == producedItemId && !r.IsDeleted)
                .OrderBy(r => r.Sequence)
                .ToListAsync();

        public async Task<RoutingStep?> GetRoutingStepAsync(int producedItemId, int stepId) =>
            await _context.RoutingSteps
                .Include(r => r.Workstation)
                .Include(r => r.Machine)
                .FirstOrDefaultAsync(r =>
                    r.Id == stepId &&
                    r.ProducedItemId == producedItemId &&
                    !r.IsDeleted);

        public async Task<bool> SequenceExistsAsync(int producedItemId, int sequence, int? excludeStepId = null) =>
            await _context.RoutingSteps
                .AnyAsync(r =>
                    r.ProducedItemId == producedItemId &&
                    r.Sequence == sequence &&
                    !r.IsDeleted &&
                    (excludeStepId == null || r.Id != excludeStepId));
    }
}
