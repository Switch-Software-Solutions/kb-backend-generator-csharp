
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Services
{
    public partial interface IAuditLogService
    {
        Task<IEnumerable<AuditLog>> GetAll();
        Task<AuditLog> GetById(int id);
        Task<AuditLog> Create(AuditLog newAuditLog, string token);
        Task<bool> IsValid(int id);
    }
}
