using WorkshopFlow.Repositories;
using WorkshopFlow.Data;

namespace WorkshopFlow.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WorkshopFlowContext _context;
        public IUserRepository UserRepository { get; }
        //public ITeacherRepository TeacherRepository { get; }
        //public IStudentRepository StudentRepository { get; }
        //public ICourseRepository CourseRepository { get; }

        public UnitOfWork(WorkshopFlowContext context)
        {
            _context = context;
            UserRepository = new UserRepository(context);
            //TeacherRepository = new TeacherRepository(context);
            //StudentRepository = new StudentRepository(context);
            //CourseRepository = new CourseRepository(context);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;   // commit & rollback
        }
    }
}
