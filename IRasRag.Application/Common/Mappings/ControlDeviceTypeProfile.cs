using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class ControlDeviceTypeProfile : Profile
    {
        public ControlDeviceTypeProfile()
        {
            // Entity to DTO (for reading)
            CreateMap<ControlDeviceType, ControlDeviceTypeDto>();

            // Create DTO to Entity (for creation)
            CreateMap<CreateControlDeviceTypeDto, ControlDeviceType>(MemberList.Source);

            // Update DTO to Entity (for updates)
            CreateMap<UpdateControlDeviceTypeDto, ControlDeviceType>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
