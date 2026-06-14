using WorkshopFlow.DTO;

namespace WorkshopFlow.Services
{
    public interface IWorkstationService
    {
        Task<IEnumerable<WorkstationReadOnlyDTO>> GetAllWorkstationsAsync();
        Task<WorkstationReadOnlyDTO> GetWorkstationByIdAsync(int id);
        Task<WorkstationReadOnlyDTO> InsertWorkstationAsync(WorkstationInsertDTO dto);
        Task<WorkstationReadOnlyDTO> UpdateWorkstationAsync(int id, WorkstationUpdateDTO dto);
        Task DeleteWorkstationAsync(int id);
    }
}
