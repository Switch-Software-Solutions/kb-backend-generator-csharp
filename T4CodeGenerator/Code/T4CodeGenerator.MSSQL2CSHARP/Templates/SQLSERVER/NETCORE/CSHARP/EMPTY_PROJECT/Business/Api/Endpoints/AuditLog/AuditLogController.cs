  

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using AutoMapper;
using Core.Services;

namespace Api.Endpoints.AuditLog
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class AuditLogController : ControllerBase
    { 
        private readonly IAuditLogService _auditLogService;
        private readonly IMapper _mapper;

        public AuditLogController(IAuditLogService auditLogService, IMapper mapper)
        {
            this._mapper = mapper;
            this._auditLogService = auditLogService;
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<Response.List.AuditLogItemList>>> GetAll()
        {
            var auditLogCollection = await _auditLogService.GetAll();

            var getAllResponse = _mapper.Map<IEnumerable<Core.Models.AuditLog>, IEnumerable<Response.List.AuditLogItemList>>(auditLogCollection);

            return Ok(getAllResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response.Read.AuditLogItem>> GetFullById(int id)
        {
            var auditLog = await _auditLogService.GetById(id);

            var getFullByIdResponse = _mapper.Map<Core.Models.AuditLog, Response.Read.AuditLogItem>(auditLog);

            return Ok(getFullByIdResponse);
        }

        [HttpGet("IsValid/{id}")]
        public async Task<ActionResult<bool>> IsValid(int id)
        {
            bool result = await _auditLogService.IsValid(id);

            return Ok(result);
        }
    } 
}

 
