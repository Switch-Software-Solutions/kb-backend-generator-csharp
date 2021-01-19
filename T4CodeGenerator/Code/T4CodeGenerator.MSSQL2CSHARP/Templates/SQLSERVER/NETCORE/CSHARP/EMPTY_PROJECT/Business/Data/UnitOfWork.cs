using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Miscellaneous;
using Core.Models;
using Core.Repositories;
using Data.Repositories;
using Newtonsoft.Json;

namespace Data
{
    public partial class UnitOfWork : IUnitOfWork
    {
        private AuditLogRepository _auditLogRepository;
    
        public IAuditLogRepository AuditLogRepository => _auditLogRepository = _auditLogRepository ?? new AuditLogRepository(_context);

        private readonly DbContextInstance _context;
        private readonly ICryptography _cryptography;

        public UnitOfWork(DbContextInstance context, ICryptography cryptography)
        {
            this._context = context;
            this._cryptography = cryptography;
        }


        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CommitAsync(string token, string eventType, string entityName, object entity)
        {
            int result;

            result = await _context.SaveChangesAsync();

            await AddAuditLog(token, eventType, entityName, entity);

            return result;
        }

        private async Task AddAuditLog(string token, string eventType, string entityName, object entity) 
        {
            AuditLog lastAuditLog = _context.AuditLogSet.Where(al => al.Id == _context.AuditLogSet.Where(al => al.Id != 0).Max(lal => lal.Id)).SingleOrDefault();
            AuditLog currentAuditLog = new AuditLog(token, eventType, entityName, entity);

            string lastAuditLogToBase64String = string.Empty;

            string json = JsonConvert.SerializeObject(lastAuditLog);

            byte[] bytes = Encoding.Default.GetBytes(json);

            lastAuditLogToBase64String = Convert.ToBase64String(bytes);

            string input = lastAuditLogToBase64String + currentAuditLog.Token + currentAuditLog.EventType + currentAuditLog.TableName + currentAuditLog.RecordValues;

            currentAuditLog.HashValue = _cryptography.GetHash(input);

            await _context.Set<AuditLog>().AddAsync(currentAuditLog);
            
            await _context.SaveChangesAsync();
        }
              

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}


