using WorkshopFlow.Services;

namespace WorkshopFlow.Services
{
    public class ApplicationService : IApplicationService
    {
        public IUserService UserService { get; }
        public IItemService ItemService { get; }

        public ApplicationService(IUserService userService, IItemService itemService)
        {
            UserService = userService;
            ItemService = itemService;
        }
            
    }
}
