using WorkshopFlow.Core;
using WorkshopFlow.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WorkshopFlow.Models;
using WorkshopFlow.Repositories;

namespace WorkshopFlow.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(WorkshopFlowContext context) : base(context)
        {
        }

        public override async Task<User?> GetByIdAsync(int id) =>
        await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);


        public async Task<User?> GetUserByUsernameAsync(string username) =>
            await _context.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.Capabilities)
                .FirstOrDefaultAsync(u =>
                    (u.Username == username || u.Email == username)
                    && !u.IsDeleted);

        public async Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize, 
            List<Expression<Func<User, bool>>> predicates)
        {
            int totalRecords;
            IQueryable<User> query = _context.Users
                .Include(u => u.Role)
                .Where(u => !u.IsDeleted);

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate); // υπονοείται το AND
                }
            }
            totalRecords = await query.CountAsync();
            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(u => u.Id) // Πάντα OrderBy για να διασφαλίσουμε την σταθερή σειρά των αποτελεσμάτων
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<User>()
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

        }
    }
}
