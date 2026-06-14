// IUnitOfWork.cs
namespace WorkshopFlow.Repositories
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IItemRepository ItemRepository { get; }
        IBomLineRepository BomLineRepository { get; }
        IWorkstationRepository WorkstationRepository { get; }
        IMachineRepository MachineRepository { get; }
        IRoutingStepRepository RoutingStepRepository { get; }
        IWorkOrderRepository WorkOrderRepository { get; }
        IWorkOrderOperationRepository WorkOrderOperationRepository { get; }
        IInventoryTransactionRepository InventoryTransactionRepository { get; }

        Task<bool> SaveAsync();
    }
}