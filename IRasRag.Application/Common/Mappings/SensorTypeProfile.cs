using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class SensorTypeProfile : Profile
    {
        public SensorTypeProfile()
        {
            // Entity to DTO (for reading)
            CreateMap<SensorType, SensorTypeDto>();

            // Create DTO to Entity (for creation)
            CreateMap<CreateSensorTypeDto, SensorType>(MemberList.Source);

            // Update DTO to Entity (for updates)
            CreateMap<UpdateSensorTypeDto, SensorType>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
