namespace WorkshopFlow.Services
{
    public interface IApplicationService
    {
        IUserService UserService { get; }
        IItemService ItemService { get; }
        IBomLineService BomLineService { get; }
        IWorkstationService WorkstationService { get; }
        IMachineService MachineService { get; }
        IRoutingStepService RoutingStepService { get; }

        // Other services can be added here
    }
}
