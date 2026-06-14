using WorkshopFlow.Models;

namespace WorkshopFlow.Repositories
{
    public interface IWorkstationRepository : IBaseRepository<Workstation>
    {
        Task<Workstation?> GetWorkstationByCodeAsync(string code);
        Task<IEnumerable<Workstation>> GetAllWorkstationsAsync();
    }
}
