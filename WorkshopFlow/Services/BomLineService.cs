using AutoMapper;
using WorkshopFlow.DTO;
using WorkshopFlow.Exceptions;
using WorkshopFlow.Models;
using WorkshopFlow.Repositories;

namespace WorkshopFlow.Services
{
    public class BomLineService : IBomLineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BomLineService> _logger;

        public BomLineService(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<BomLineService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BomLineReadOnlyDTO>> GetBomByItemIdAsync(int producedItemId)
        {
            // Επιχειρησιακός κανόνας: το item πρέπει να υπάρχει
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(producedItemId)
                ?? throw new EntityNotFoundException("Item",
                    $"Item with id {producedItemId} not found");

            // Επιχειρησιακός κανόνας: μόνο παραγόμενα items έχουν BOM
            if (!item.IsManufactured)
            {
                throw new InvalidArgumentException("Item",
                    $"Item with id {producedItemId} is not a manufactured item and cannot have a BOM");
            }

            var bomLines = await _unitOfWork.BomLineRepository
                .GetBomByProducedItemIdAsync(producedItemId);

            _logger.LogInformation("Retrieved BOM for item with id {Id}", producedItemId);
            return _mapper.Map<IEnumerable<BomLineReadOnlyDTO>>(bomLines);
        }

        public async Task<BomLineReadOnlyDTO> InsertBomLineAsync(int producedItemId, BomLineInsertDTO dto)
        {
            // Επιχειρησιακός κανόνας: το produced item πρέπει να υπάρχει
            var producedItem = await _unitOfWork.ItemRepository.GetByIdAsync(producedItemId)
                ?? throw new EntityNotFoundException("Item",
                    $"Item with id {producedItemId} not found");

            // Επιχειρησιακός κανόνας: μόνο παραγόμενα items έχουν BOM
            if (!producedItem.IsManufactured)
            {
                throw new InvalidArgumentException("Item",
                    $"Item with id {producedItemId} is not a manufactured item and cannot have a BOM");
            }

            // Επιχειρησιακός κανόνας: το component item πρέπει να υπάρχει
            var componentItem = await _unitOfWork.ItemRepository.GetByIdAsync(dto.ComponentItemId!.Value)
                ?? throw new EntityNotFoundException("Item",
                    $"Component item with id {dto.ComponentItemId} not found");

            // Επιχειρησιακός κανόνας: circular reference check
            if (dto.ComponentItemId == producedItemId)
            {
                throw new InvalidArgumentException("BomLine",
                    "An item cannot be a component of itself");
            }

            // Επιχειρησιακός κανόνας: FinalProduct δεν μπορεί να είναι component
            if (componentItem.ItemType == ItemType.FinalProduct)
            {
                throw new InvalidArgumentException("BomLine",
                    $"A Final Product cannot be used as a component in a BOM");
            }

            // Επιχειρησιακός κανόνας: το component δεν μπορεί να εμφανίζεται δύο φορές
            var exists = await _unitOfWork.BomLineRepository
                .ComponentExistsInBomAsync(producedItemId, dto.ComponentItemId!.Value);
            if (exists)
            {
                throw new EntityAlreadyExistsException("BomLine",
                    $"Component item with id {dto.ComponentItemId} already exists in this BOM");
            }

            var bomLine = _mapper.Map<BomLine>(dto);
            bomLine.ProducedItemId = producedItemId;

            await _unitOfWork.BomLineRepository.AddAsync(bomLine);
            await _unitOfWork.SaveAsync();

            // Reload για να πάρουμε τα navigation properties
            var createdBomLine = await _unitOfWork.BomLineRepository
                .GetBomLineAsync(producedItemId, bomLine.Id);

            _logger.LogInformation("BomLine added to item {Id}", producedItemId);
            return _mapper.Map<BomLineReadOnlyDTO>(createdBomLine);
        }

        public async Task<BomLineReadOnlyDTO> UpdateBomLineAsync(int producedItemId,
            int bomLineId, BomLineUpdateDTO dto)
        {
            var bomLine = await _unitOfWork.BomLineRepository
                .GetBomLineAsync(producedItemId, bomLineId)
                ?? throw new EntityNotFoundException("BomLine",
                    $"BomLine with id {bomLineId} not found for item {producedItemId}");

            _mapper.Map(dto, bomLine);
            bomLine.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.BomLineRepository.UpdateAsync(bomLine);
            await _unitOfWork.SaveAsync();

            var updatedBomLine = await _unitOfWork.BomLineRepository
                .GetBomLineAsync(producedItemId, bomLineId);

            _logger.LogInformation("BomLine {BomLineId} updated for item {ItemId}",
                bomLineId, producedItemId);
            return _mapper.Map<BomLineReadOnlyDTO>(updatedBomLine);
        }

        public async Task DeleteBomLineAsync(int producedItemId, int bomLineId)
        {
            var bomLine = await _unitOfWork.BomLineRepository
                .GetBomLineAsync(producedItemId, bomLineId)
                ?? throw new EntityNotFoundException("BomLine",
                    $"BomLine with id {bomLineId} not found for item {producedItemId}");

            // Soft delete
            bomLine.IsDeleted = true;
            bomLine.DeletedAt = DateTime.UtcNow;
            bomLine.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.BomLineRepository.UpdateAsync(bomLine);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("BomLine {BomLineId} deleted from item {ItemId}",
                bomLineId, producedItemId);
        }
    }
}

