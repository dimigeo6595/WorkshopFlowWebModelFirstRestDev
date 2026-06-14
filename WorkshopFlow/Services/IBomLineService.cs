using WorkshopFlow.DTO;

namespace WorkshopFlow.Services
{
    public interface IBomLineService
    {
        Task<IEnumerable<BomLineReadOnlyDTO>> GetBomByItemIdAsync(int producedItemId);
        Task<BomLineReadOnlyDTO> InsertBomLineAsync(int producedItemId, BomLineInsertDTO dto);
        Task<BomLineReadOnlyDTO> UpdateBomLineAsync(int producedItemId, int bomLineId, BomLineUpdateDTO dto);
        Task DeleteBomLineAsync(int producedItemId, int bomLineId);
    }
}
