namespace WorkshopFlow.Services
{
    public class ApplicationService : IApplicationService
    {
        public IUserService UserService { get; }
        public IItemService ItemService { get; }
        public IBomLineService BomLineService { get; }

        public ApplicationService(
            IUserService userService,
            IItemService itemService,
            IBomLineService bomLineService)
        {
            UserService = userService;
            ItemService = itemService;
            BomLineService = bomLineService;
        }
    }
}
