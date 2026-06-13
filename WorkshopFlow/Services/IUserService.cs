using WorkshopFlow.Core;
using WorkshopFlow.Models;
using WorkshopFlow.Core.Filters;
using WorkshopFlow.DTO;


namespace WorkshopFlow.Services
{
    public interface IUserService
    {
        Task<User> VerifyAndGetUserAsync(UserLoginDTO credentials);
        Task<UserReadOnlyDTO> GetUserByUsernameAsync(string username);
        Task<UserReadOnlyDTO> GetUserByIdAsync(int id);

        Task<UserReadOnlyDTO> InsertUserAsync(UserInsertDTO dto);
        Task<UserReadOnlyDTO> UpdateUserAsync(int id, UserUpdateDTO dto);
        Task PatchUserAsync(int id, UserPatchDTO dto);
        Task DeleteUserAsync(int id);
        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, 
            int pageSize, UserFiltersDTO userFiltersDTO);
        string CreateUserToken(User user);
    }
}
