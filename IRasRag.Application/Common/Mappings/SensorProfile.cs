using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class SensorProfile : Profile
    {
        public SensorProfile()
        {
            CreateMap<Sensor, SensorDto>()
                .ForMember(
                    dest => dest.SensorTypeName,
                    opt => opt.MapFrom(src => src.SensorType.Name)
                )
                .ForMember(
                    dest => dest.MasterBoardName,
                    opt => opt.MapFrom(src => src.MasterBoard.Name)
                );

            CreateMap<CreateSensorDto, Sensor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.SensorType, opt => opt.Ignore())
                .ForMember(dest => dest.MasterBoard, opt => opt.Ignore())
                .ForMember(dest => dest.SensorLogs, opt => opt.Ignore())
                .ForMember(dest => dest.Jobs, opt => opt.Ignore())
                .ValidateMemberList(MemberList.Source);

            CreateMap<UpdateSensorDto, Sensor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.SensorType, opt => opt.Ignore())
                .ForMember(dest => dest.MasterBoard, opt => opt.Ignore())
                .ForMember(dest => dest.SensorLogs, opt => opt.Ignore())
                .ForMember(dest => dest.Jobs, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
