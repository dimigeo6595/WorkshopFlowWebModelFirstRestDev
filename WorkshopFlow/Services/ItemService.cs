using AutoMapper;
using WorkshopFlow.Core;
using WorkshopFlow.Core.Filters;
using WorkshopFlow.DTO;
using WorkshopFlow.Exceptions;
using WorkshopFlow.Models;
using WorkshopFlow.Repositories;
using System.Linq.Expressions;

namespace WorkshopFlow.Services
{
    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ItemService> _logger;

        public ItemService(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<ItemService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ItemReadOnlyDTO> GetItemByIdAsync(int id)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Item", $"Item with id {id} not found");

            _logger.LogInformation("Item with id {Id} found", id);
            return _mapper.Map<ItemReadOnlyDTO>(item);
        }

        public async Task<ItemReadOnlyDTO> GetItemByCodeAsync(string itemCode)
        {
            var item = await _unitOfWork.ItemRepository.GetItemByCodeAsync(itemCode)
                ?? throw new EntityNotFoundException("Item", $"Item with code {itemCode} not found");

            _logger.LogInformation("Item with code {ItemCode} found", itemCode);
            return _mapper.Map<ItemReadOnlyDTO>(item);
        }

        public async Task<PaginatedResult<ItemReadOnlyDTO>> GetPaginatedItemsFilteredAsync(
            int pageNumber, int pageSize, ItemFiltersDTO filters)
        {
            List<Expression<Func<Item, bool>>> predicates = [];

            if (!string.IsNullOrEmpty(filters.Name))
            {
                predicates.Add(i => i.Name.Contains(filters.Name));
            }
            if (filters.ItemType.HasValue)
            {
                predicates.Add(i => i.ItemType == filters.ItemType.Value);
            }

            var result = await _unitOfWork.ItemRepository.GetItemsAsync(
                pageNumber, pageSize, predicates, filters.SortBy, filters.SortDescending);

            var dtoResult = new PaginatedResult<ItemReadOnlyDTO>()
            {
                Data = _mapper.Map<List<ItemReadOnlyDTO>>(result.Data),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };

            _logger.LogInformation("Retrieved {Count} items", dtoResult.Data.Count);
            return dtoResult;
        }

        public async Task<ItemReadOnlyDTO> InsertItemAsync(ItemInsertDTO dto)
        {
            // Επιχειρησιακός κανόνας: ο ItemCode πρέπει να είναι μοναδικός
            var existingItem = await _unitOfWork.ItemRepository.GetItemByCodeAsync(dto.ItemCode!);
            if (existingItem != null)
            {
                throw new EntityAlreadyExistsException("Item",
                    $"Item with code {dto.ItemCode} already exists");
            }

            var item = _mapper.Map<Item>(dto);
            await _unitOfWork.ItemRepository.AddAsync(item);
            await _unitOfWork.SaveAsync();

            // Reload για να πάρουμε τα navigation properties (UnitOfMeasure, WeightUoM)
            var createdItem = await _unitOfWork.ItemRepository.GetByIdAsync(item.Id);

            _logger.LogInformation("Item {ItemCode} created successfully", item.ItemCode);
            return _mapper.Map<ItemReadOnlyDTO>(createdItem);
        }

        public async Task<ItemReadOnlyDTO> UpdateItemAsync(int id, ItemUpdateDTO dto)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Item", $"Item with id {id} not found");

            // Επιχειρησιακός κανόνας: δεν αλλάζουμε StockQuantity εδώ
            _mapper.Map(dto, item);
            item.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.ItemRepository.UpdateAsync(item);
            await _unitOfWork.SaveAsync();

            var updatedItem = await _unitOfWork.ItemRepository.GetByIdAsync(id);

            _logger.LogInformation("Item with id {Id} updated successfully", id);
            return _mapper.Map<ItemReadOnlyDTO>(updatedItem);
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Item", $"Item with id {id} not found");

            // Soft delete
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            item.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.ItemRepository.UpdateAsync(item);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Item with id {Id} soft deleted", id);
        }

        public async Task<ItemReadOnlyDTO> CalculateWeightAsync(int id)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Item", $"Item with id {id} not found");

            // Επιχειρησιακός κανόνας: μόνο παραγόμενα items έχουν βάρος από BOM
            if (!item.IsManufactured)
            {
                throw new InvalidArgumentException("Item",
                    "Weight calculation is only available for manufactured items (SemiFinished, FinalProduct)");
            }

            // Φέρε τις γραμμές BOM
            var bomLines = await _unitOfWork.BomLineRepository
                .GetBomByProducedItemIdAsync(id);

            if (!bomLines.Any())
            {
                throw new InvalidArgumentException("Item",
                    $"Item with id {id} has no BOM lines. Cannot calculate weight.");
            }

            // Επιχειρησιακός κανόνας: όλα τα components πρέπει να έχουν WeightPerUoM
            var missingWeight = bomLines
                .Where(b => !b.ComponentItem.WeightPerUoM.HasValue)
                .Select(b => b.ComponentItem.ItemCode)
                .ToList();

            if (missingWeight.Any())
            {
                throw new InvalidArgumentException("Item",
                    $"Cannot calculate weight. Missing WeightPerUoM for components: {string.Join(", ", missingWeight)}");
            }

            // weight = Σ (BomLine.Quantity × Component.WeightPerUoM)
            var totalWeight = bomLines.Sum(b => b.Quantity * b.ComponentItem.WeightPerUoM!.Value);

            item.Weight = totalWeight;
            item.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.ItemRepository.UpdateAsync(item);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Weight calculated for item {Id}: {Weight} kg", id, totalWeight);

            var updatedItem = await _unitOfWork.ItemRepository.GetByIdAsync(id);
            return _mapper.Map<ItemReadOnlyDTO>(updatedItem);
        }

    }
}
