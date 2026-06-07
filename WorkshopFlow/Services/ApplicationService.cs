using WorkshopFlow.Services;

namespace WorkshopFlow.Services
{
    public class ApplicationService : IApplicationService
    {
        public IUserService UserService { get; }

        public ApplicationService(IUserService userService)
        {
            UserService = userService;
        }
            
    }
}
