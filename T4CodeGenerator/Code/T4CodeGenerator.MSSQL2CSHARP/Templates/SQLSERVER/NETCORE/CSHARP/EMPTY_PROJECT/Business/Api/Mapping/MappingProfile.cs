using AutoMapper;

namespace Api.Mapping
{
    public partial class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to Resource
            CreateMap<Core.Models.AuditLog, Endpoints.AuditLog.Response.Read.AuditLogItem>();
            CreateMap<Core.Models.AuditLog, Endpoints.AuditLog.Response.List.AuditLogItemList>();
                        
            // Resource to Domain
        }
    }
}

