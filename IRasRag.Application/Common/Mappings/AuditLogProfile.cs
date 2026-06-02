using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class AuditLogProfile : Profile
    {
        public AuditLogProfile()
        {
            CreateMap<AuditLog, AuditLogDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()))
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId.ToString()))
                .ForMember(d => d.EntityId, o => o.MapFrom(s => s.EntityId))
                // Role được set thủ công sau khi load User→Role trong AuditLogService
                .ForMember(d => d.Role, o => o.Ignore());
        }
    }
}
