using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using WorkshopFlow.DTO;
using WorkshopFlow.Exceptions;
using WorkshopFlow.Models;
using WorkshopFlow.Models.Enums;
using WorkshopFlow.Repositories;
using WorkshopFlow.Services;
using Xunit;

namespace WorkshopFlow.Tests.Services
{
    public class WorkOrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IItemRepository> _itemRepositoryMock;
        private readonly Mock<IBomLineRepository> _bomLineRepositoryMock;
        private readonly Mock<IRoutingStepRepository> _routingStepRepositoryMock;
        private readonly Mock<IWorkOrderRepository> _workOrderRepositoryMock;
        private readonly Mock<IWorkOrderOperationRepository> _operationRepositoryMock;
        private readonly Mock<IInventoryTransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<WorkOrderService>> _loggerMock;
        private readonly WorkOrderService _sut;

        public WorkOrderServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _itemRepositoryMock = new Mock<IItemRepository>();
            _bomLineRepositoryMock = new Mock<IBomLineRepository>();
            _routingStepRepositoryMock = new Mock<IRoutingStepRepository>();
            _workOrderRepositoryMock = new Mock<IWorkOrderRepository>();
            _operationRepositoryMock = new Mock<IWorkOrderOperationRepository>();
            _transactionRepositoryMock = new Mock<IInventoryTransactionRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<WorkOrderService>>();

            _unitOfWorkMock.Setup(u => u.ItemRepository).Returns(_itemRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.BomLineRepository).Returns(_bomLineRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.RoutingStepRepository).Returns(_routingStepRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.WorkOrderRepository).Returns(_workOrderRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.WorkOrderOperationRepository).Returns(_operationRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.InventoryTransactionRepository).Returns(_transactionRepositoryMock.Object);

            _sut = new WorkOrderService(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        // ─── InsertWorkOrderAsync ────────────────────────────────────────────

        [Fact]
        public async Task InsertWorkOrderAsync_WhenItemDoesNotExist_ThrowsEntityNotFoundException()
        {
            var dto = new WorkOrderInsertDTO
            {
                ProducedItemId = 999,
                Quantity = 5,
                PlannedStartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.AddDays(7)
            };

            _itemRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Item?)null);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _sut.InsertWorkOrderAsync(dto, createdByUserId: 1));
        }

        [Fact]
        public async Task InsertWorkOrderAsync_WhenItemIsRawMaterial_ThrowsInvalidArgumentException()
        {
            var dto = new WorkOrderInsertDTO
            {
                ProducedItemId = 1,
                Quantity = 5,
                PlannedStartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.AddDays(7)
            };
            var rawMaterial = new Item { Id = 1, ItemCode = "RM-001", ItemType = ItemType.RawMaterial };

            _itemRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(rawMaterial);

            await Assert.ThrowsAsync<InvalidArgumentException>(
                () => _sut.InsertWorkOrderAsync(dto, createdByUserId: 1));
        }

        [Fact]
        public async Task InsertWorkOrderAsync_WhenItemHasNoBom_ThrowsInvalidArgumentException()
        {
            var dto = new WorkOrderInsertDTO
            {
                ProducedItemId = 2,
                Quantity = 5,
                PlannedStartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.AddDays(7)
            };
            var item = new Item { Id = 2, ItemCode = "SF-001", ItemType = ItemType.SemiFinished };

            _itemRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(item);
            _bomLineRepositoryMock
                .Setup(r => r.GetBomByProducedItemIdAsync(2))
                .ReturnsAsync(new List<BomLine>());

            await Assert.ThrowsAsync<InvalidArgumentException>(
                () => _sut.InsertWorkOrderAsync(dto, createdByUserId: 1));
        }

        [Fact]
        public async Task InsertWorkOrderAsync_WhenItemHasNoRouting_ThrowsInvalidArgumentException()
        {
            var dto = new WorkOrderInsertDTO
            {
                ProducedItemId = 3,
                Quantity = 5,
                PlannedStartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.AddDays(7)
            };
            var item = new Item { Id = 3, ItemCode = "SF-002", ItemType = ItemType.SemiFinished };
            var bomLines = new List<BomLine> { new() { Id = 1 } };

            _itemRepositoryMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(item);
            _bomLineRepositoryMock.Setup(r => r.GetBomByProducedItemIdAsync(3)).ReturnsAsync(bomLines);
            _routingStepRepositoryMock
                .Setup(r => r.GetRoutingByProducedItemIdAsync(3))
                .ReturnsAsync(new List<RoutingStep>());

            await Assert.ThrowsAsync<InvalidArgumentException>(
                () => _sut.InsertWorkOrderAsync(dto, createdByUserId: 1));
        }

        [Fact]
        public async Task InsertWorkOrderAsync_WhenEndDateBeforeStartDate_ThrowsInvalidArgumentException()
        {
            var dto = new WorkOrderInsertDTO
            {
                ProducedItemId = 4,
                Quantity = 5,
                PlannedStartDate = DateTime.Today.AddDays(10),
                PlannedEndDate = DateTime.Today.AddDays(5) // ΠΡΙΝ το start!
            };
            var item = new Item { Id = 4, ItemCode = "SF-003", ItemType = ItemType.SemiFinished };
            var bomLines = new List<BomLine> { new() { Id = 1 } };
            var routingSteps = new List<RoutingStep> { new() { Id = 1 } };

            _itemRepositoryMock.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(item);
            _bomLineRepositoryMock.Setup(r => r.GetBomByProducedItemIdAsync(4)).ReturnsAsync(bomLines);
            _routingStepRepositoryMock.Setup(r => r.GetRoutingByProducedItemIdAsync(4)).ReturnsAsync(routingSteps);

            await Assert.ThrowsAsync<InvalidArgumentException>(
                () => _sut.InsertWorkOrderAsync(dto, createdByUserId: 1));
        }

        // ─── CancelWorkOrderAsync ─────────────────────────────────────────────

        [Fact]
        public async Task CancelWorkOrderAsync_WhenWorkOrderDoesNotExist_ThrowsEntityNotFoundException()
        {
            _workOrderRepositoryMock
                .Setup(r => r.GetWorkOrderWithDetailsAsync(999))
                .ReturnsAsync((WorkOrder?)null);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _sut.CancelWorkOrderAsync(999));
        }

        [Fact]
        public async Task CancelWorkOrderAsync_WhenWorkOrderIsCompleted_ThrowsInvalidArgumentException()
        {
            var workOrder = new WorkOrder
            {
                Id = 1,
                WorkOrderCode = "WO-001",
                Status = WorkOrderStatus.Completed
            };

            _workOrderRepositoryMock
                .Setup(r => r.GetWorkOrderWithDetailsAsync(1))
                .ReturnsAsync(workOrder);

            await Assert.ThrowsAsync<InvalidArgumentException>(
                () => _sut.CancelWorkOrderAsync(1));
        }

        [Fact]
        public async Task CancelWorkOrderAsync_WhenWorkOrderIsDraft_SetsStatusToCancelled()
        {
            // Draft WorkOrder μπορεί να ακυρωθεί — δεν χρειάζεται stock επιστροφή
            var workOrder = new WorkOrder
            {
                Id = 3,
                WorkOrderCode = "WO-003",
                Status = WorkOrderStatus.Draft,
                Operations = new List<WorkOrderOperation>()
            };

            _workOrderRepositoryMock
                .Setup(r => r.GetWorkOrderWithDetailsAsync(3))
                .ReturnsAsync(workOrder);
            _workOrderRepositoryMock.Setup(r => r.UpdateAsync(workOrder)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(true);

            await _sut.CancelWorkOrderAsync(3);

            Assert.Equal(WorkOrderStatus.Cancelled, workOrder.Status);
        }

        // ─── GetWorkOrderByIdAsync ────────────────────────────────────────────

        [Fact]
        public async Task GetWorkOrderByIdAsync_WhenWorkOrderExists_ReturnsDto()
        {
            var workOrder = new WorkOrder
            {
                Id = 1,
                WorkOrderCode = "WO-20260726-100",
                Status = WorkOrderStatus.Draft
            };
            var dto = new WorkOrderReadOnlyDTO
            {
                Id = 1,
                WorkOrderCode = "WO-20260726-100"
            };

            _workOrderRepositoryMock
                .Setup(r => r.GetWorkOrderWithDetailsAsync(1))
                .ReturnsAsync(workOrder);
            _mapperMock.Setup(m => m.Map<WorkOrderReadOnlyDTO>(workOrder)).Returns(dto);

            var result = await _sut.GetWorkOrderByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("WO-20260726-100", result.WorkOrderCode);
        }

        [Fact]
        public async Task GetWorkOrderByIdAsync_WhenWorkOrderDoesNotExist_ThrowsEntityNotFoundException()
        {
            _workOrderRepositoryMock
                .Setup(r => r.GetWorkOrderWithDetailsAsync(999))
                .ReturnsAsync((WorkOrder?)null);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _sut.GetWorkOrderByIdAsync(999));
        }

        // ─── StartOperationAsync ──────────────────────────────────────────────

        [Fact]
        public async Task StartOperationAsync_WhenOperationIsNotPending_ThrowsInvalidArgumentException()
        {
            // Arrange — operation είναι ήδη InProgress
            var operation = new WorkOrderOperation
            {
                Id = 10,
                WorkOrderId = 1,
                Sequence = 1,
                Status = WorkOrderOperationStatus.InProgress,
                AssignedToUserId = 5
            };

            // Το service καλεί GetOperationAsync(workOrderId, operationId) — όχι GetByIdAsync
            _operationRepositoryMock
                .Setup(r => r.GetOperationAsync(1, 10))
                .ReturnsAsync(operation);

            await Assert.ThrowsAsync<InvalidArgumentException>(
                () => _sut.StartOperationAsync(workOrderId: 1, operationId: 10));
        }

        [Fact]
        public async Task StartOperationAsync_WhenOperationHasNoAssignedOperator_ThrowsInvalidArgumentException()
        {
            // Arrange — Pending αλλά χωρίς assigned operator
            var operation = new WorkOrderOperation
            {
                Id = 11,
                WorkOrderId = 1,
                Sequence = 1,
                Status = WorkOrderOperationStatus.Pending,
                AssignedToUserId = null // ΔΕΝ έχει assigned
            };

            _operationRepositoryMock
                .Setup(r => r.GetOperationAsync(1, 11))
                .ReturnsAsync(operation);

            await Assert.ThrowsAsync<InvalidArgumentException>(
                () => _sut.StartOperationAsync(workOrderId: 1, operationId: 11));
        }
    }
}
