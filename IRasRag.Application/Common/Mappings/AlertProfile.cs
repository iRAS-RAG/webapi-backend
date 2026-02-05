using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Common.Mappings
{
    public class AlertProfile : Profile
    {
        public AlertProfile()
        {
            // Alert -> AlertDto
            CreateMap<Alert, AlertDto>()
                .ForMember(dest => dest.FishTankName, opt => opt.MapFrom(src => src.FishTank.Name))
                .ForMember(
                    dest => dest.SensorTypeName,
                    opt => opt.MapFrom(src => src.SensorType.Name)
                )
                .ForMember(
                    dest => dest.FarmingBatchName,
                    opt =>
                        opt.MapFrom(src => src.FarmingBatch != null ? src.FarmingBatch.Name : null)
                );

            // CreateAlertDto -> Alert
            CreateMap<CreateAlertDto, Alert>(MemberList.Source)
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => AlertStatus.OPEN));

            // UpdateAlertDto -> Alert
            CreateMap<UpdateAlertDto, Alert>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
