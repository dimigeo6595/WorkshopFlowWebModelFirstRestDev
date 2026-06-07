
using System.Linq.Expressions;
using WorkshopFlow.Core;
using WorkshopFlow.Core.Entities;
using WorkshopFlow.Repositories;

namespace WorkshopFlow.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates);
    }
}
