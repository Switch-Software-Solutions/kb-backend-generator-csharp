using System;
using System.Threading.Tasks;
using Core.Repositories;

namespace Core
{
    public partial interface IUnitOfWork : IDisposable
    {
        IAuditLogRepository AuditLogRepository { get; }

        Task<int> CommitAsync();

        Task<int> CommitAsync(string token, string eventType, string entityName, object entity);
    }
}

