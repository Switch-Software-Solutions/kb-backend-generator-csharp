using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoreAuth.Models;
using CoreAuth.Repositories;
using DataAuth;

namespace DataAuth.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AuthDbContext context)
            : base(context)
        { }

        public async Task<User> FindCompleteAsync(Expression<System.Func<User, bool>> predicate)
        {
            return await TemplateDbContext.UserSet.Include(u => u.RefreshTokens)
                .Include(u => u.RecoveryKeys)
                .Where(predicate)
                .SingleOrDefaultAsync();
        }

        public async Task<User> GetByIdCompleteAsync(int userId)
        {
            return await TemplateDbContext.UserSet.Include(u => u.RefreshTokens)
                .Include(u => u.RecoveryKeys)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        private AuthDbContext TemplateDbContext
        {
            get { return Context as AuthDbContext; }
        }
    }
}
