using WorkshopFlow.DTO;

namespace WorkshopFlow.Services
{
    public interface IMachineService
    {
        Task<IEnumerable<MachineReadOnlyDTO>> GetMachinesByWorkstationAsync(int workstationId);
        Task<MachineReadOnlyDTO> GetMachineByIdAsync(int id);
        Task<MachineReadOnlyDTO> InsertMachineAsync(int workstationId, MachineInsertDTO dto);
        Task<MachineReadOnlyDTO> UpdateMachineAsync(int workstationId, int id, MachineUpdateDTO dto);
        Task DeleteMachineAsync(int workstationId, int id);
    }
}
