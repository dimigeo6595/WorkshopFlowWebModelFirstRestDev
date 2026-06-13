using WorkshopFlow.Core;
using WorkshopFlow.Core.Filters;
using WorkshopFlow.DTO;

namespace WorkshopFlow.Services
{
    public interface IItemService
    {
        Task<ItemReadOnlyDTO> GetItemByIdAsync(int id);
        Task<ItemReadOnlyDTO> GetItemByCodeAsync(string itemCode);
        Task<PaginatedResult<ItemReadOnlyDTO>> GetPaginatedItemsFilteredAsync(
            int pageNumber, int pageSize, ItemFiltersDTO filters);
        Task<ItemReadOnlyDTO> InsertItemAsync(ItemInsertDTO dto);
        Task<ItemReadOnlyDTO> UpdateItemAsync(int id, ItemUpdateDTO dto);
        Task DeleteItemAsync(int id);
        Task<ItemReadOnlyDTO> CalculateWeightAsync(int id);
    }
}
