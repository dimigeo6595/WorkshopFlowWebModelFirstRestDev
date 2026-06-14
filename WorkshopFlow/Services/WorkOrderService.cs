using AutoMapper;
using System.Linq.Expressions;
using WorkshopFlow.Core;
using WorkshopFlow.Core.Filters;
using WorkshopFlow.DTO;
using WorkshopFlow.Exceptions;
using WorkshopFlow.Models;
using WorkshopFlow.Models.Enums;
using WorkshopFlow.Repositories;

namespace WorkshopFlow.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<WorkOrderService> _logger;

        public WorkOrderService(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<WorkOrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<WorkOrderReadOnlyDTO>> GetPaginatedWorkOrdersAsync(
            int pageNumber, int pageSize, WorkOrderFiltersDTO filters)
        {
            List<Expression<Func<WorkOrder, bool>>> predicates = [];

            if (filters.Status.HasValue)
            {
                predicates.Add(w => w.Status == filters.Status.Value);
            }
            if (filters.ProducedItemId.HasValue)
            {
                predicates.Add(w => w.ProducedItemId == filters.ProducedItemId.Value);
            }

            var result = await _unitOfWork.WorkOrderRepository
                .GetWorkOrdersAsync(pageNumber, pageSize, predicates);

            return new PaginatedResult<WorkOrderReadOnlyDTO>
            {
                Data = _mapper.Map<List<WorkOrderReadOnlyDTO>>(result.Data),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        public async Task<WorkOrderReadOnlyDTO> GetWorkOrderByIdAsync(int id)
        {
            var workOrder = await _unitOfWork.WorkOrderRepository.GetWorkOrderWithDetailsAsync(id)
                ?? throw new EntityNotFoundException("WorkOrder",
                    $"WorkOrder with id {id} not found");

            _logger.LogInformation("WorkOrder with id {Id} found", id);
            return _mapper.Map<WorkOrderReadOnlyDTO>(workOrder);
        }

        public async Task<WorkOrderReadOnlyDTO> InsertWorkOrderAsync(
    WorkOrderInsertDTO dto, int createdByUserId)
        {
            // Επιχειρησιακός κανόνας: το item πρέπει να υπάρχει
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(dto.ProducedItemId!.Value)
                ?? throw new EntityNotFoundException("Item",
                    $"Item with id {dto.ProducedItemId} not found");

            // Επιχειρησιακός κανόνας: μόνο παραγόμενα items μπορούν να έχουν WorkOrder
            if (!item.IsManufactured)
            {
                throw new InvalidArgumentException("Item",
                    $"Item with id {dto.ProducedItemId} is not a manufactured item");
            }

            // Επιχειρησιακός κανόνας: το item πρέπει να έχει BOM
            var bomLines = await _unitOfWork.BomLineRepository
                .GetBomByProducedItemIdAsync(dto.ProducedItemId!.Value);
            if (!bomLines.Any())
            {
                throw new InvalidArgumentException("WorkOrder",
                    $"Item with id {dto.ProducedItemId} has no BOM defined");
            }

            // Επιχειρησιακός κανόνας: το item πρέπει να έχει Routing
            var routingSteps = await _unitOfWork.RoutingStepRepository
                .GetRoutingByProducedItemIdAsync(dto.ProducedItemId!.Value);
            if (!routingSteps.Any())
            {
                throw new InvalidArgumentException("WorkOrder",
                    $"Item with id {dto.ProducedItemId} has no Routing defined");
            }

            // Επιχειρησιακός κανόνας: PlannedEndDate πρέπει να είναι μετά το PlannedStartDate
            if (dto.PlannedEndDate <= dto.PlannedStartDate)
            {
                throw new InvalidArgumentException("WorkOrder",
                    "PlannedEndDate must be after PlannedStartDate");
            }

            // Auto-generate WorkOrderCode: WO-YYYYMMDD-XXX
            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            var count = await _unitOfWork.WorkOrderRepository.GetCountAsync();
            var workOrderCode = $"WO-{today}-{(count + 1):D3}";

            var workOrder = _mapper.Map<WorkOrder>(dto);
            workOrder.WorkOrderCode = workOrderCode;
            workOrder.Status = WorkOrderStatus.Draft;
            workOrder.CreatedByUserId = createdByUserId;

            await _unitOfWork.WorkOrderRepository.AddAsync(workOrder);
            await _unitOfWork.SaveAsync();

            var createdWorkOrder = await _unitOfWork.WorkOrderRepository
                .GetWorkOrderWithDetailsAsync(workOrder.Id);

            _logger.LogInformation("WorkOrder {Code} created successfully", workOrderCode);
            return _mapper.Map<WorkOrderReadOnlyDTO>(createdWorkOrder);
        }

        public async Task<WorkOrderReadOnlyDTO> UpdateWorkOrderAsync(int id, WorkOrderUpdateDTO dto)
        {
            var workOrder = await _unitOfWork.WorkOrderRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("WorkOrder",
                    $"WorkOrder with id {id} not found");

            // Επιχειρησιακός κανόνας: μόνο Draft WorkOrders μπορούν να ενημερωθούν
            if (workOrder.Status != WorkOrderStatus.Draft)
            {
                throw new InvalidArgumentException("WorkOrder",
                    $"Only Draft WorkOrders can be updated. Current status: {workOrder.Status}");
            }

            // Επιχειρησιακός κανόνας: PlannedEndDate πρέπει να είναι μετά το PlannedStartDate
            if (dto.PlannedEndDate <= dto.PlannedStartDate)
            {
                throw new InvalidArgumentException("WorkOrder",
                    "PlannedEndDate must be after PlannedStartDate");
            }

            _mapper.Map(dto, workOrder);
            workOrder.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.WorkOrderRepository.UpdateAsync(workOrder);
            await _unitOfWork.SaveAsync();

            var updatedWorkOrder = await _unitOfWork.WorkOrderRepository
                .GetWorkOrderWithDetailsAsync(id);

            _logger.LogInformation("WorkOrder {Id} updated successfully", id);
            return _mapper.Map<WorkOrderReadOnlyDTO>(updatedWorkOrder);
        }

        public async Task DeleteWorkOrderAsync(int id)
        {
            var workOrder = await _unitOfWork.WorkOrderRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("WorkOrder",
                    $"WorkOrder with id {id} not found");

            // Επιχειρησιακός κανόνας: μόνο Draft WorkOrders μπορούν να διαγραφούν
            if (workOrder.Status != WorkOrderStatus.Draft)
            {
                throw new InvalidArgumentException("WorkOrder",
                    $"Only Draft WorkOrders can be deleted. Current status: {workOrder.Status}");
            }

            // Soft delete
            workOrder.IsDeleted = true;
            workOrder.DeletedAt = DateTime.UtcNow;
            workOrder.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.WorkOrderRepository.UpdateAsync(workOrder);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("WorkOrder {Id} soft deleted", id);
        }

        public async Task<WorkOrderReadOnlyDTO> ReleaseWorkOrderAsync(int id)
        {
            var workOrder = await _unitOfWork.WorkOrderRepository.GetWorkOrderWithDetailsAsync(id)
                ?? throw new EntityNotFoundException("WorkOrder",
                    $"WorkOrder with id {id} not found");

            // Επιχειρησιακός κανόνας: μόνο Draft WorkOrders μπορούν να γίνουν Released
            if (workOrder.Status != WorkOrderStatus.Draft)
            {
                throw new InvalidArgumentException("WorkOrder",
                    $"Only Draft WorkOrders can be released. Current status: {workOrder.Status}");
            }

            // Φέρε BOM lines και Routing steps
            var bomLines = await _unitOfWork.BomLineRepository
                .GetBomByProducedItemIdAsync(workOrder.ProducedItemId);
            var routingSteps = await _unitOfWork.RoutingStepRepository
                .GetRoutingByProducedItemIdAsync(workOrder.ProducedItemId);

            // Επιχειρησιακός κανόνας: έλεγχος stock για κάθε component
            foreach (var bomLine in bomLines)
            {
                var requiredQuantity = bomLine.Quantity * workOrder.Quantity;
                var component = await _unitOfWork.ItemRepository
                    .GetByIdAsync(bomLine.ComponentItemId)
                    ?? throw new EntityNotFoundException("Item",
                        $"Component item with id {bomLine.ComponentItemId} not found");

                if (component.StockQuantity < requiredQuantity)
                {
                    throw new InsufficientStockException(
                        component.ItemCode,
                        requiredQuantity,
                        component.StockQuantity);
                }

            }

            // Δημιουργία WorkOrderOperations από το Routing
            foreach (var step in routingSteps)
            {
                var operation = new WorkOrderOperation
                {
                    WorkOrderId = workOrder.Id,
                    RoutingStepId = step.Id,
                    Sequence = step.Sequence,
                    Status = WorkOrderOperationStatus.Pending,
                    InsertedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };
                await _unitOfWork.WorkOrderOperationRepository.AddAsync(operation);
            }

            // Κατανάλωση stock για κάθε component μέσω InventoryTransaction
            foreach (var bomLine in bomLines)
            {
                var requiredQuantity = bomLine.Quantity * workOrder.Quantity;
                var component = await _unitOfWork.ItemRepository
                    .GetByIdAsync(bomLine.ComponentItemId);

                // Δημιουργία Consumption transaction
                var transaction = new InventoryTransaction
                {
                    ItemId = bomLine.ComponentItemId,
                    WorkOrderId = workOrder.Id,
                    TransactionType = TransactionType.Consumption,
                    // Αρνητικό γιατί καταναλώνουμε stock
                    Quantity = -requiredQuantity,
                    CreatedByUserId = workOrder.CreatedByUserId,
                    Notes = $"Consumption for WorkOrder {workOrder.WorkOrderCode}",
                    InsertedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };
                await _unitOfWork.InventoryTransactionRepository.AddAsync(transaction);

                // Μείωση StockQuantity
                component!.StockQuantity -= requiredQuantity;
                component.ModifiedAt = DateTime.UtcNow;
                await _unitOfWork.ItemRepository.UpdateAsync(component);
            }

            // Status transition: Draft → Released
            workOrder.Status = WorkOrderStatus.Released;
            workOrder.ModifiedAt = DateTime.UtcNow;
            await _unitOfWork.WorkOrderRepository.UpdateAsync(workOrder);

            await _unitOfWork.SaveAsync();

            var releasedWorkOrder = await _unitOfWork.WorkOrderRepository
                .GetWorkOrderWithDetailsAsync(id);

            _logger.LogInformation("WorkOrder {Code} released successfully", workOrder.WorkOrderCode);
            return _mapper.Map<WorkOrderReadOnlyDTO>(releasedWorkOrder);
        }

        public async Task CancelWorkOrderAsync(int id)
        {
            var workOrder = await _unitOfWork.WorkOrderRepository.GetWorkOrderWithDetailsAsync(id)
                ?? throw new EntityNotFoundException("WorkOrder",
                    $"WorkOrder with id {id} not found");

            // Επιχειρησιακός κανόνας: δεν μπορείς να ακυρώσεις Completed WorkOrder
            if (workOrder.Status == WorkOrderStatus.Completed)
            {
                throw new InvalidArgumentException("WorkOrder",
                    "Cannot cancel a Completed WorkOrder");
            }

            // Αν Released ή InProgress → επιστροφή consumed stock
            if (workOrder.Status == WorkOrderStatus.Released ||
                workOrder.Status == WorkOrderStatus.InProgress)
            {
                var bomLines = await _unitOfWork.BomLineRepository
                    .GetBomByProducedItemIdAsync(workOrder.ProducedItemId);

                foreach (var bomLine in bomLines)
                {
                    var requiredQuantity = bomLine.Quantity * workOrder.Quantity;
                    var component = await _unitOfWork.ItemRepository
                        .GetByIdAsync(bomLine.ComponentItemId);

                    // Δημιουργία Adjustment transaction για επιστροφή stock
                    var transaction = new InventoryTransaction
                    {
                        ItemId = bomLine.ComponentItemId,
                        WorkOrderId = workOrder.Id,
                        TransactionType = TransactionType.Adjustment,
                        // Θετικό γιατί επιστρέφουμε stock
                        Quantity = requiredQuantity,
                        CreatedByUserId = workOrder.CreatedByUserId,
                        Notes = $"Stock return due to cancellation of WorkOrder {workOrder.WorkOrderCode}",
                        InsertedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.InventoryTransactionRepository.AddAsync(transaction);

                    // Επιστροφή StockQuantity
                    component!.StockQuantity += requiredQuantity;
                    component.ModifiedAt = DateTime.UtcNow;
                    await _unitOfWork.ItemRepository.UpdateAsync(component);
                }
            }

            // Ακύρωση όλων των pending/inprogress operations
            foreach (var operation in workOrder.Operations
                .Where(o => !o.IsDeleted &&
                    o.Status != WorkOrderOperationStatus.Completed))
            {
                operation.Status = WorkOrderOperationStatus.Cancelled;
                operation.ModifiedAt = DateTime.UtcNow;
                await _unitOfWork.WorkOrderOperationRepository.UpdateAsync(operation);
            }

            // Status transition → Cancelled
            workOrder.Status = WorkOrderStatus.Cancelled;
            workOrder.ModifiedAt = DateTime.UtcNow;
            await _unitOfWork.WorkOrderRepository.UpdateAsync(workOrder);

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("WorkOrder {Code} cancelled", workOrder.WorkOrderCode);
        }

        public async Task<IEnumerable<WorkOrderOperationReadOnlyDTO>> GetOperationsAsync(int workOrderId)
        {
            var workOrder = await _unitOfWork.WorkOrderRepository.GetByIdAsync(workOrderId)
                ?? throw new EntityNotFoundException("WorkOrder",
                    $"WorkOrder with id {workOrderId} not found");

            var operations = await _unitOfWork.WorkOrderOperationRepository
                .GetOperationsByWorkOrderAsync(workOrderId);

            _logger.LogInformation("Retrieved operations for WorkOrder {Id}", workOrderId);
            return _mapper.Map<IEnumerable<WorkOrderOperationReadOnlyDTO>>(operations);
        }

        public async Task<WorkOrderOperationReadOnlyDTO> AssignOperationAsync(
            int workOrderId, int operationId, WorkOrderOperationAssignDTO dto)
        {
            var operation = await _unitOfWork.WorkOrderOperationRepository
                .GetOperationAsync(workOrderId, operationId)
                ?? throw new EntityNotFoundException("WorkOrderOperation",
                    $"Operation with id {operationId} not found for WorkOrder {workOrderId}");

            // Επιχειρησιακός κανόνας: μόνο Pending operations μπορούν να γίνουν assigned
            if (operation.Status != WorkOrderOperationStatus.Pending)
            {
                throw new InvalidArgumentException("WorkOrderOperation",
                    $"Only Pending operations can be assigned. Current status: {operation.Status}");
            }

            // Επιχειρησιακός κανόνας: ο user πρέπει να υπάρχει και να έχει role OPERATOR
            var user = await _unitOfWork.UserRepository.GetByIdAsync(dto.AssignedToUserId!.Value)
                ?? throw new EntityNotFoundException("User",
                    $"User with id {dto.AssignedToUserId} not found");

            if (user.Role.Name != "OPERATOR")
            {
                throw new InvalidArgumentException("User",
                    $"Only OPERATOR users can be assigned to operations");
            }

            operation.AssignedToUserId = dto.AssignedToUserId!.Value;
            operation.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.WorkOrderOperationRepository.UpdateAsync(operation);
            await _unitOfWork.SaveAsync();

            var updatedOperation = await _unitOfWork.WorkOrderOperationRepository
                .GetOperationAsync(workOrderId, operationId);

            _logger.LogInformation("Operation {OpId} assigned to user {UserId}",
                operationId, dto.AssignedToUserId);
            return _mapper.Map<WorkOrderOperationReadOnlyDTO>(updatedOperation);
        }

        public async Task<WorkOrderOperationReadOnlyDTO> StartOperationAsync(
            int workOrderId, int operationId)
        {
            var operation = await _unitOfWork.WorkOrderOperationRepository
                .GetOperationAsync(workOrderId, operationId)
                ?? throw new EntityNotFoundException("WorkOrderOperation",
                    $"Operation with id {operationId} not found for WorkOrder {workOrderId}");

            // Επιχειρησιακός κανόνας: μόνο Pending operations μπορούν να ξεκινήσουν
            if (operation.Status != WorkOrderOperationStatus.Pending)
            {
                throw new InvalidArgumentException("WorkOrderOperation",
                    $"Only Pending operations can be started. Current status: {operation.Status}");
            }

            // Επιχειρησιακός κανόνας: η operation πρέπει να είναι assigned
            if (!operation.AssignedToUserId.HasValue)
            {
                throw new InvalidArgumentException("WorkOrderOperation",
                    "Operation must be assigned to an operator before it can be started");
            }

            // Επιχειρησιακός κανόνας: η προηγούμενη operation πρέπει να είναι Completed
            if (operation.Sequence > 1)
            {
                var previousOperation = await _unitOfWork.WorkOrderOperationRepository
                    .GetPreviousOperationAsync(workOrderId, operation.Sequence);

                if (previousOperation == null ||
                    previousOperation.Status != WorkOrderOperationStatus.Completed)
                {
                    throw new InvalidArgumentException("WorkOrderOperation",
                        $"Previous operation (sequence {operation.Sequence - 1}) " +
                        "must be completed before starting this operation");
                }
            }

            operation.Status = WorkOrderOperationStatus.InProgress;
            operation.ActualStartDate = DateTime.UtcNow;
            operation.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.WorkOrderOperationRepository.UpdateAsync(operation);

            // Αν είναι η πρώτη operation -> WorkOrder status = InProgress
            if (operation.Sequence == 1)
            {
                var workOrder = await _unitOfWork.WorkOrderRepository.GetByIdAsync(workOrderId);
                if (workOrder != null && workOrder.Status == WorkOrderStatus.Released)
                {
                    workOrder.Status = WorkOrderStatus.InProgress;
                    workOrder.ActualStartDate = DateTime.UtcNow;
                    workOrder.ModifiedAt = DateTime.UtcNow;
                    await _unitOfWork.WorkOrderRepository.UpdateAsync(workOrder);
                }
            }

            await _unitOfWork.SaveAsync();

            var updatedOperation = await _unitOfWork.WorkOrderOperationRepository
                .GetOperationAsync(workOrderId, operationId);

            _logger.LogInformation("Operation {OpId} started for WorkOrder {WoId}",
                operationId, workOrderId);
            return _mapper.Map<WorkOrderOperationReadOnlyDTO>(updatedOperation);
        }

        public async Task<WorkOrderOperationReadOnlyDTO> CompleteOperationAsync(
            int workOrderId, int operationId)
        {
            var operation = await _unitOfWork.WorkOrderOperationRepository
                .GetOperationAsync(workOrderId, operationId)
                ?? throw new EntityNotFoundException("WorkOrderOperation",
                    $"Operation with id {operationId} not found for WorkOrder {workOrderId}");

            // Επιχειρησιακός κανόνας: μόνο InProgress operations μπορούν να ολοκληρωθούν
            if (operation.Status != WorkOrderOperationStatus.InProgress)
            {
                throw new InvalidArgumentException("WorkOrderOperation",
                    $"Only InProgress operations can be completed. Current status: {operation.Status}");
            }

            operation.Status = WorkOrderOperationStatus.Completed;
            operation.ActualEndDate = DateTime.UtcNow;
            operation.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.WorkOrderOperationRepository.UpdateAsync(operation);

            // Έλεγχος αν όλες οι operations είναι Completed
            var allOperations = await _unitOfWork.WorkOrderOperationRepository
                .GetOperationsByWorkOrderAsync(workOrderId);

            // Ενημέρωσε την τρέχουσα operation στη λίστα
            var allCompleted = allOperations
                .Where(o => o.Id != operationId)
                .All(o => o.Status == WorkOrderOperationStatus.Completed);

            if (allCompleted)
            {
                var workOrder = await _unitOfWork.WorkOrderRepository
                    .GetWorkOrderWithDetailsAsync(workOrderId);

                if (workOrder != null)
                {
                    // Status transition -> Completed
                    workOrder.Status = WorkOrderStatus.Completed;
                    workOrder.ActualEndDate = DateTime.UtcNow;
                    workOrder.ModifiedAt = DateTime.UtcNow;
                    await _unitOfWork.WorkOrderRepository.UpdateAsync(workOrder);

                    // Production transaction — αύξηση stock παραγόμενου item
                    var productionTransaction = new InventoryTransaction
                    {
                        ItemId = workOrder.ProducedItemId,
                        WorkOrderId = workOrder.Id,
                        TransactionType = TransactionType.Production,
                        // Θετικό γιατί παράγουμε stock
                        Quantity = workOrder.Quantity,
                        CreatedByUserId = workOrder.CreatedByUserId,
                        Notes = $"Production completed for WorkOrder {workOrder.WorkOrderCode}",
                        InsertedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.InventoryTransactionRepository.AddAsync(productionTransaction);

                    // Αύξηση StockQuantity παραγόμενου item
                    var producedItem = await _unitOfWork.ItemRepository
                        .GetByIdAsync(workOrder.ProducedItemId);
                    if (producedItem != null)
                    {
                        producedItem.StockQuantity += workOrder.Quantity;
                        producedItem.ModifiedAt = DateTime.UtcNow;
                        await _unitOfWork.ItemRepository.UpdateAsync(producedItem);
                    }

                    _logger.LogInformation("WorkOrder {Code} completed. Stock updated for item {ItemId}",
                        workOrder.WorkOrderCode, workOrder.ProducedItemId);
                }
            }

            await _unitOfWork.SaveAsync();

            var updatedOperation = await _unitOfWork.WorkOrderOperationRepository
                .GetOperationAsync(workOrderId, operationId);

            _logger.LogInformation("Operation {OpId} completed for WorkOrder {WoId}",
                operationId, workOrderId);
            return _mapper.Map<WorkOrderOperationReadOnlyDTO>(updatedOperation);
        }
    }
}
