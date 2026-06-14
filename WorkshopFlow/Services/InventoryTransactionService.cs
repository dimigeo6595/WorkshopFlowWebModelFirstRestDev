using AutoMapper;
using WorkshopFlow.DTO;
using WorkshopFlow.Exceptions;
using WorkshopFlow.Models;
using WorkshopFlow.Models.Enums;
using WorkshopFlow.Repositories;

namespace WorkshopFlow.Services
{
    public class InventoryTransactionService : IInventoryTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<InventoryTransactionService> _logger;

        public InventoryTransactionService(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<InventoryTransactionService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<InventoryTransactionReadOnlyDTO>> GetTransactionsByItemAsync(
            int itemId)
        {
            // Επιχειρησιακός κανόνας: το item πρέπει να υπάρχει
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(itemId)
                ?? throw new EntityNotFoundException("Item",
                    $"Item with id {itemId} not found");

            var transactions = await _unitOfWork.InventoryTransactionRepository
                .GetTransactionsByItemAsync(itemId);

            _logger.LogInformation("Retrieved {Count} transactions for item {Id}",
                transactions.Count(), itemId);
            return _mapper.Map<IEnumerable<InventoryTransactionReadOnlyDTO>>(transactions);
        }

        public async Task<IEnumerable<InventoryTransactionReadOnlyDTO>> GetTransactionsByWorkOrderAsync(
            int workOrderId)
        {
            // Επιχειρησιακός κανόνας: το WorkOrder πρέπει να υπάρχει
            var workOrder = await _unitOfWork.WorkOrderRepository.GetByIdAsync(workOrderId)
                ?? throw new EntityNotFoundException("WorkOrder",
                    $"WorkOrder with id {workOrderId} not found");

            var transactions = await _unitOfWork.InventoryTransactionRepository
                .GetTransactionsByWorkOrderAsync(workOrderId);

            _logger.LogInformation("Retrieved {Count} transactions for WorkOrder {Id}",
                transactions.Count(), workOrderId);
            return _mapper.Map<IEnumerable<InventoryTransactionReadOnlyDTO>>(transactions);
        }

        public async Task<InventoryTransactionReadOnlyDTO> InsertManualTransactionAsync(
            InventoryTransactionInsertDTO dto, int createdByUserId)
        {
            // Επιχειρησιακός κανόνας: μόνο Purchase και Adjustment επιτρέπονται manual
            // Production και Consumption δημιουργούνται αυτόματα από WorkOrder
            if (dto.TransactionType != TransactionType.Purchase &&
                dto.TransactionType != TransactionType.Adjustment)
            {
                throw new InvalidArgumentException("InventoryTransaction",
                    "Only Purchase and Adjustment transactions can be created manually. " +
                    "Production and Consumption are created automatically by WorkOrders.");
            }

            // Επιχειρησιακός κανόνας: το item πρέπει να υπάρχει
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(dto.ItemId!.Value)
                ?? throw new EntityNotFoundException("Item",
                    $"Item with id {dto.ItemId} not found");

            // Επιχειρησιακός κανόνας: το stock δεν μπορεί να γίνει αρνητικό
            // Ισχύει μόνο για Adjustment με αρνητική ποσότητα
            if (dto.TransactionType == TransactionType.Adjustment &&
                dto.Quantity < 0 &&
                item.StockQuantity + dto.Quantity < 0)
            {
                throw new InvalidArgumentException("InventoryTransaction",
                    $"Adjustment would result in negative stock for item {item.ItemCode}. " +
                    $"Current stock: {item.StockQuantity}, Adjustment: {dto.Quantity}");
            }

            var transaction = _mapper.Map<InventoryTransaction>(dto);
            transaction.CreatedByUserId = createdByUserId;
            transaction.InsertedAt = DateTime.UtcNow;
            transaction.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.InventoryTransactionRepository.AddAsync(transaction);

            // Ενημέρωση StockQuantity του item
            item.StockQuantity += dto.Quantity!.Value;
            item.ModifiedAt = DateTime.UtcNow;
            await _unitOfWork.ItemRepository.UpdateAsync(item);

            await _unitOfWork.SaveAsync();

            // Reload για navigation properties
            var createdTransaction = await _unitOfWork.InventoryTransactionRepository
                .GetByIdAsync(transaction.Id);

            _logger.LogInformation("{TransactionType} transaction created for item {ItemCode}. " +
                "Quantity: {Quantity}. New stock: {Stock}",
                dto.TransactionType, item.ItemCode, dto.Quantity, item.StockQuantity);

            return _mapper.Map<InventoryTransactionReadOnlyDTO>(createdTransaction);
        }
    }
}
