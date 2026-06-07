using WorkshopFlow.Core;
using WorkshopFlow.Core.Entities;
using WorkshopFlow.Core.Filters;
using WorkshopFlow.DTO;


namespace WorkshopFlow  .Services
{
    public interface IUserService
    {
        Task<User> VerifyAndGetUserAsync(UserLoginDTO credentials);
        Task<UserReadOnlyDTO> GetUserByUsernameAsync(string username);
        Task<UserReadOnlyDTO> GetUserByIdAsync(int id);
        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, 
            int pageSize, UserFiltersDTO userFiltersDTO);
        string CreateUserToken(User user);
    }
}
