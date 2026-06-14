using WorkshopFlow.Models;

namespace WorkshopFlow.Repositories
{
    public interface IMachineRepository : IBaseRepository<Machine>
    {
        Task<Machine?> GetMachineByCodeAsync(string code);
        Task<IEnumerable<Machine>> GetMachinesByWorkstationAsync(int workstationId);
    }
}
