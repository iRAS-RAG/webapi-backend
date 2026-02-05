using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class CameraProfile : Profile
    {
        public CameraProfile()
        {
            // Entity to DTO (for reading)
            CreateMap<Camera, CameraDto>();

            // Create DTO to Entity (for creation)
            CreateMap<CreateCameraDto, Camera>(MemberList.Source);

            // Update DTO to Entity (for updates)
            CreateMap<UpdateCameraDto, Camera>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
