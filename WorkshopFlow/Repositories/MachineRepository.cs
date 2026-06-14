using Microsoft.EntityFrameworkCore;
using WorkshopFlow.Data;
using WorkshopFlow.Models;

namespace WorkshopFlow.Repositories
{
    public class MachineRepository : BaseRepository<Machine>, IMachineRepository
    {
        public MachineRepository(WorkshopFlowContext context) : base(context)
        {
        }

        public override async Task<Machine?> GetByIdAsync(int id) =>
            await _context.Machines
                .Include(m => m.Workstation)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

        public async Task<Machine?> GetMachineByCodeAsync(string code) =>
            await _context.Machines
                .Include(m => m.Workstation)
                .FirstOrDefaultAsync(m => m.Code == code && !m.IsDeleted);

        public async Task<IEnumerable<Machine>> GetMachinesByWorkstationAsync(int workstationId) =>
            await _context.Machines
                .Include(m => m.Workstation)
                .Where(m => m.WorkstationId == workstationId && !m.IsDeleted)
                .OrderBy(m => m.Code)
                .ToListAsync();
    }
}
