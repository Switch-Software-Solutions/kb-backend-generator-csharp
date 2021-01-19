using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreAuth.Models;

namespace CoreAuth.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> FindCompleteAsync(Expression<System.Func<User, bool>> predicate);

        Task<User> GetByIdCompleteAsync(int userId);
    }
}