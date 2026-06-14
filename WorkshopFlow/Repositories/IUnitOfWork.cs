using WorkshopFlow.Repositories;

namespace WorkshopFlow.Repositories
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IItemRepository ItemRepository { get; }
        IBomLineRepository BomLineRepository { get; }

        Task<bool> SaveAsync();
    }
}
