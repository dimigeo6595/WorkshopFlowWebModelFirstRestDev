using WorkshopFlow.Repositories;

namespace WorkshopFlow.Repositories
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        
        Task<bool> SaveAsync();
    }
}
