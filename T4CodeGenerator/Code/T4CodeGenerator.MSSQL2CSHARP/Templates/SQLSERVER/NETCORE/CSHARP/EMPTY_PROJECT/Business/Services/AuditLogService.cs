
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Miscellaneous;
using Core.Models;
using Core.Services;
using Newtonsoft.Json;

namespace Services
{
    public partial class AuditLogService : IAuditLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICryptography _cryptography;
        public AuditLogService(IUnitOfWork unitOfWork, ICryptography cryptography)
        {
            this._unitOfWork = unitOfWork;
            this._cryptography = cryptography;
        }

        public Task<AuditLog> Create(AuditLog newAuditLog, string token)
        {
            throw new InvalidOperationException("Invalid operation");
        }

        public Task Delete(AuditLog auditLog, string token)
        {
            throw  new InvalidOperationException("Invalid operation");
        }

        public async Task<IEnumerable<AuditLog>> GetAll()
        {
            return await _unitOfWork.AuditLogRepository.GetAllAsync();
        }

        public async Task<AuditLog> GetById(int id)
        {
            return await _unitOfWork.AuditLogRepository.GetByIdAsync(id);
        }

        public Task Update(AuditLog auditLogToBeUpdated, AuditLog auditLog, string token)
        {
            throw new InvalidOperationException("Invalid operation");
        }

        public async Task<bool> IsValid(int id)
        {
            bool result = false;

            var auditLogList = await _unitOfWork.AuditLogRepository.GetByIdAndPreviousAsync(id, 2);

            AuditLog currentAuditLog = auditLogList.ElementAt(0);
            AuditLog previousAuditLog = auditLogList.ElementAt(1);

            string lastAuditLogToBase64String = string.Empty;

            string json = JsonConvert.SerializeObject(previousAuditLog);

            byte[] bytes = Encoding.Default.GetBytes(json);

            lastAuditLogToBase64String = Convert.ToBase64String(bytes);

            string input = lastAuditLogToBase64String + currentAuditLog.Token + currentAuditLog.EventType + currentAuditLog.TableName + currentAuditLog.RecordValues;

            result = _cryptography.VerifyHash(input, currentAuditLog.HashValue);

            return result;
        }
    }
}

