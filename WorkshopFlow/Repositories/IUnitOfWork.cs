using WorkshopFlow.Repositories;

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


        Task<bool> SaveAsync();
    }
}
