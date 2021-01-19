
using System.Threading.Tasks;
using CoreAuth;
using CoreAuth.Repositories;
using DataAuth.Repositories;

namespace DataAuth
{
    public partial class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDbContext _context;

        private UserRepository _userRepository;

        public UnitOfWork(AuthDbContext context)
        {
            this._context = context;
        }

        public IUserRepository UserRepository => _userRepository = _userRepository ?? new UserRepository(_context);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
