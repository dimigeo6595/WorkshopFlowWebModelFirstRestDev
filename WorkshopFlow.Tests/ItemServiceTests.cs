using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using WorkshopFlow.DTO;
using WorkshopFlow.Exceptions;
using WorkshopFlow.Models;
using WorkshopFlow.Repositories;
using WorkshopFlow.Services;
using Xunit;

namespace WorkshopFlow.Tests.Services
{
    public class ItemServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IItemRepository> _itemRepositoryMock;
        private readonly Mock<IBomLineRepository> _bomLineRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ItemService>> _loggerMock;
        private readonly ItemService _sut;

        public ItemServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _itemRepositoryMock = new Mock<IItemRepository>();
            _bomLineRepositoryMock = new Mock<IBomLineRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ItemService>>();

            _unitOfWorkMock.Setup(u => u.ItemRepository).Returns(_itemRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.BomLineRepository).Returns(_bomLineRepositoryMock.Object);

            _sut = new ItemService(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        // ─── GetItemByIdAsync ───────────────────────────────────────────────

        [Fact]
        public async Task GetItemByIdAsync_WhenItemExists_ReturnsDto()
        {
            var item = new Item { Id = 1, ItemCode = "RM-001", Name = "Steel Wire" };
            var dto = new ItemReadOnlyDTO { Id = 1, ItemCode = "RM-001", Name = "Steel Wire" };

            _itemRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
            _mapperMock.Setup(m => m.Map<ItemReadOnlyDTO>(item)).Returns(dto);

            var result = await _sut.GetItemByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("RM-001", result.ItemCode);
        }

        [Fact]
        public async Task GetItemByIdAsync_WhenItemDoesNotExist_ThrowsEntityNotFoundException()
        {
            _itemRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Item?)null);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _sut.GetItemByIdAsync(999));
        }

        // ─── InsertItemAsync ────────────────────────────────────────────────

        [Fact]
        public async Task InsertItemAsync_WhenItemCodeIsUnique_CreatesItem()
        {
            var dto = new ItemInsertDTO
            {
                ItemCode = "RM-NEW-001",
                Name = "New Material",
                ItemType = ItemType.RawMaterial,
                UnitOfMeasureId = 1
            };
            var item = new Item { Id = 10, ItemCode = "RM-NEW-001", Name = "New Material" };
            var resultDto = new ItemReadOnlyDTO { Id = 10, ItemCode = "RM-NEW-001", Name = "New Material" };

            _itemRepositoryMock
                .Setup(r => r.GetItemByCodeAsync("RM-NEW-001"))
                .ReturnsAsync((Item?)null);

            _mapperMock.Setup(m => m.Map<Item>(dto)).Returns(item);
            _itemRepositoryMock.Setup(r => r.AddAsync(item)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(true);
            _itemRepositoryMock.Setup(r => r.GetByIdAsync(item.Id)).ReturnsAsync(item);
            _mapperMock.Setup(m => m.Map<ItemReadOnlyDTO>(item)).Returns(resultDto);

            var result = await _sut.InsertItemAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("RM-NEW-001", result.ItemCode);
            _itemRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Item>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task InsertItemAsync_WhenItemCodeAlreadyExists_ThrowsEntityAlreadyExistsException()
        {
            var dto = new ItemInsertDTO
            {
                ItemCode = "RM-001",
                Name = "Duplicate Item",
                ItemType = ItemType.RawMaterial,
                UnitOfMeasureId = 1
            };
            var existingItem = new Item { Id = 1, ItemCode = "RM-001" };

            _itemRepositoryMock
                .Setup(r => r.GetItemByCodeAsync("RM-001"))
                .ReturnsAsync(existingItem);

            await Assert.ThrowsAsync<EntityAlreadyExistsException>(
                () => _sut.InsertItemAsync(dto));

            _itemRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Item>()), Times.Never);
        }

        // ─── DeleteItemAsync ────────────────────────────────────────────────

        [Fact]
        public async Task DeleteItemAsync_WhenItemExists_SoftDeletesItem()
        {
            var item = new Item { Id = 5, ItemCode = "RM-001", IsDeleted = false };

            _itemRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(item);
            _itemRepositoryMock.Setup(r => r.UpdateAsync(item)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(true);

            await _sut.DeleteItemAsync(5);

            Assert.True(item.IsDeleted);
            Assert.NotNull(item.DeletedAt);
            _itemRepositoryMock.Verify(r => r.UpdateAsync(item), Times.Once);
        }

        [Fact]
        public async Task DeleteItemAsync_WhenItemDoesNotExist_ThrowsEntityNotFoundException()
        {
            _itemRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Item?)null);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _sut.DeleteItemAsync(999));
        }

        // ─── CalculateWeightAsync ───────────────────────────────────────────

        [Fact]
        public async Task CalculateWeightAsync_WhenItemIsNotManufactured_ThrowsInvalidArgumentException()
        {
            var rawMaterial = new Item
            {
                Id = 1,
                ItemCode = "RM-001",
                ItemType = ItemType.RawMaterial
            };

            _itemRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(rawMaterial);

            await Assert.ThrowsAsync<InvalidArgumentException>(
                () => _sut.CalculateWeightAsync(1));
        }

        [Fact]
        public async Task CalculateWeightAsync_WhenItemHasNoBom_ThrowsInvalidArgumentException()
        {
            var item = new Item
            {
                Id = 2,
                ItemCode = "SF-001",
                ItemType = ItemType.SemiFinished
            };

            _itemRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(item);
            _bomLineRepositoryMock
                .Setup(r => r.GetBomByProducedItemIdAsync(2))
                .ReturnsAsync(new List<BomLine>());

            await Assert.ThrowsAsync<InvalidArgumentException>(
                () => _sut.CalculateWeightAsync(2));
        }

        [Fact]
        public async Task CalculateWeightAsync_WhenComponentMissingWeight_ThrowsInvalidArgumentException()
        {
            var item = new Item { Id = 3, ItemCode = "SF-002", ItemType = ItemType.SemiFinished };
            var componentWithoutWeight = new Item { ItemCode = "RM-NO-WEIGHT", WeightPerUoM = null };
            var bomLines = new List<BomLine>
            {
                new() { Quantity = 2, ComponentItem = componentWithoutWeight }
            };

            _itemRepositoryMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(item);
            _bomLineRepositoryMock.Setup(r => r.GetBomByProducedItemIdAsync(3)).ReturnsAsync(bomLines);

            await Assert.ThrowsAsync<InvalidArgumentException>(
                () => _sut.CalculateWeightAsync(3));
        }

        [Fact]
        public async Task CalculateWeightAsync_WhenAllComponentsHaveWeight_CalculatesCorrectly()
        {
            // Arrange
            var item = new Item { Id = 4, ItemCode = "SF-003", ItemType = ItemType.SemiFinished };
            var component1 = new Item { ItemCode = "RM-001", WeightPerUoM = 2.0m };
            var component2 = new Item { ItemCode = "RM-002", WeightPerUoM = 3.0m };
            var bomLines = new List<BomLine>
            {
                new() { Quantity = 2, ComponentItem = component1 },  // 2 × 2.0 = 4.0
                new() { Quantity = 1, ComponentItem = component2 },  // 1 × 3.0 = 3.0
            };
            var updatedItem = new Item { Id = 4, Weight = 7.0m, ItemType = ItemType.SemiFinished };
            var dto = new ItemReadOnlyDTO { Id = 4 };

            // SetupSequence: πρώτη κλήση επιστρέφει item, δεύτερη επιστρέφει updatedItem
            _itemRepositoryMock
                .SetupSequence(r => r.GetByIdAsync(4))
                .ReturnsAsync(item)
                .ReturnsAsync(updatedItem);

            _bomLineRepositoryMock.Setup(r => r.GetBomByProducedItemIdAsync(4)).ReturnsAsync(bomLines);
            _itemRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Item>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<ItemReadOnlyDTO>(updatedItem)).Returns(dto);

            // Act
            await _sut.CalculateWeightAsync(4);

            // Assert: weight = 4.0 + 3.0 = 7.0
            Assert.Equal(7.0m, item.Weight);
        }
    }
}
