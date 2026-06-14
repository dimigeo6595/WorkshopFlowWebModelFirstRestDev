namespace WorkshopFlow.Services
{
    public class ApplicationService : IApplicationService
    {
        public IUserService UserService { get; }
        public IItemService ItemService { get; }
        public IBomLineService BomLineService { get; }
        public IWorkstationService WorkstationService { get; }
        public IMachineService MachineService { get; }
        public IRoutingStepService RoutingStepService { get; }
        public IWorkOrderService WorkOrderService { get; }
        public IInventoryTransactionService InventoryTransactionService { get; }

        public ApplicationService(
            IUserService userService,
            IItemService itemService,
            IBomLineService bomLineService,
            IWorkstationService workstationService,
            IMachineService machineService,
            IRoutingStepService routingStepService,
            IWorkOrderService workOrderService,
            IInventoryTransactionService inventoryTransactionService)
        {
            UserService = userService;
            ItemService = itemService;
            BomLineService = bomLineService;
            WorkstationService = workstationService;
            MachineService = machineService;
            RoutingStepService = routingStepService;
            WorkOrderService = workOrderService;
            InventoryTransactionService = inventoryTransactionService;
        }
    }
}
