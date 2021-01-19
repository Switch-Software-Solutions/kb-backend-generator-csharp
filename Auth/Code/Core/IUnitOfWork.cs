
using System;
using System.Threading.Tasks;
using CoreAuth.Repositories;

namespace CoreAuth
{
    public partial interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }

        Task<int> CommitAsync();
    }
}
