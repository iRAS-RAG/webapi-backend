using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class JobControlMappingProfile : Profile
    {
        public JobControlMappingProfile()
        {
            // Entity to DTO
            CreateMap<JobControlMapping, JobControlMappingDto>()
                .ForMember(dest => dest.JobName, opt => opt.MapFrom(src => src.Job.Name))
                .ForMember(
                    dest => dest.ControlDeviceName,
                    opt => opt.MapFrom(src => src.ControlDevice.Name)
                );

            // Create DTO to Entity
            CreateMap<CreateJobControlMappingDto, JobControlMapping>(MemberList.Source);

            // Update DTO to Entity
            CreateMap<UpdateJobControlMappingDto, JobControlMapping>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
