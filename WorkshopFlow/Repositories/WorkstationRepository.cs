using Microsoft.EntityFrameworkCore;
using WorkshopFlow.Data;
using WorkshopFlow.Models;

namespace WorkshopFlow.Repositories
{
    public class WorkstationRepository : BaseRepository<Workstation>, IWorkstationRepository
    {
        public WorkstationRepository(WorkshopFlowContext context) : base(context)
        {
        }

        public override async Task<Workstation?> GetByIdAsync(int id) =>
            await _context.Workstations
                .Include(w => w.Machines.Where(m => !m.IsDeleted))
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);

        public async Task<Workstation?> GetWorkstationByCodeAsync(string code) =>
            await _context.Workstations
                .FirstOrDefaultAsync(w => w.Code == code && !w.IsDeleted);

        public async Task<IEnumerable<Workstation>> GetAllWorkstationsAsync() =>
            await _context.Workstations
                .Include(w => w.Machines.Where(m => !m.IsDeleted))
                .Where(w => !w.IsDeleted)
                .OrderBy(w => w.Id)
                .ToListAsync();
    }
}
