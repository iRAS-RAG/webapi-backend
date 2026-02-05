using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class ControlDeviceProfile : Profile
    {
        public ControlDeviceProfile()
        {
            // Entity to DTO
            CreateMap<ControlDevice, ControlDeviceDto>()
                .ForMember(
                    dest => dest.MasterBoardName,
                    opt =>
                        opt.MapFrom(src =>
                            src.MasterBoard != null ? src.MasterBoard.Name : string.Empty
                        )
                )
                .ForMember(
                    dest => dest.ControlDeviceTypeName,
                    opt =>
                        opt.MapFrom(src =>
                            src.ControlDeviceType != null
                                ? src.ControlDeviceType.Name
                                : string.Empty
                        )
                );

            // Create DTO to Entity
            CreateMap<CreateControlDeviceDto, ControlDevice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.MasterBoard, opt => opt.Ignore())
                .ForMember(dest => dest.ControlDeviceType, opt => opt.Ignore())
                .ForMember(dest => dest.JobControlMappings, opt => opt.Ignore());

            // Update DTO to Entity (not commonly used, but included for completeness)
            CreateMap<UpdateControlDeviceDto, ControlDevice>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
