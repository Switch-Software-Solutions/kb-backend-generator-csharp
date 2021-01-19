using Core.Models;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    internal partial class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(DbContextInstance context) 
            : base(context)
        { }
     
        private DbContextInstance TemplateDbContext
        {
            get { return Context as DbContextInstance; }
        }

        public override async ValueTask<AuditLog> GetFullByIdAsync(int id)
        {
            return await Context.Set<AuditLog>()
                .Where(auditLog => auditLog.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByIdAndPreviousAsync(int id, int take) 
        {
            return await Context.Set<AuditLog>().Where(al => al.Id <= id).OrderByDescending(al => al.Id).Take(take).ToListAsync();
        }
    }
}
