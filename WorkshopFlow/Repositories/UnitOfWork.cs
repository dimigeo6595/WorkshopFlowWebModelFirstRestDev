using WorkshopFlow.Data;

namespace WorkshopFlow.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WorkshopFlowContext _context;
        public IUserRepository UserRepository { get; }
        public IItemRepository ItemRepository { get; }
        public IBomLineRepository BomLineRepository { get; }
        public IWorkstationRepository WorkstationRepository { get; }
        public IMachineRepository MachineRepository { get; }
        public IRoutingStepRepository RoutingStepRepository { get; }

        public UnitOfWork(WorkshopFlowContext context)
        {
            _context = context;
            UserRepository = new UserRepository(context);
            ItemRepository = new ItemRepository(context);
            BomLineRepository = new BomLineRepository(context);
            WorkstationRepository = new WorkstationRepository(context);
            MachineRepository = new MachineRepository(context);
            RoutingStepRepository = new RoutingStepRepository(context);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0; //commit and rollback
        }
    }
}