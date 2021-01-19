
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Repositories
{
    public partial interface IAuditLogRepository : IRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetByIdAndPreviousAsync(int id, int take);
    }
}
