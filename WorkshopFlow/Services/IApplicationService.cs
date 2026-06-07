using WorkshopFlow.Services;

namespace WorkshopFlow.Services
{
    public interface IApplicationService
    {
        IUserService UserService { get; }
        

        // Other services can be added here 
    }
}
