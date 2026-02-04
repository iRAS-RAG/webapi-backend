using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class JobProfile : Profile
    {
        public JobProfile()
        {
            // Entity to DTO
            CreateMap<Job, JobDto>()
                .ForMember(
                    dest => dest.JobTypeName,
                    opt => opt.MapFrom(src => src.JobType != null ? src.JobType.Name : string.Empty)
                )
                .ForMember(
                    dest => dest.SensorName,
                    opt => opt.MapFrom(src => src.Sensor != null ? src.Sensor.Name : null)
                );

            // Create DTO to Entity
            CreateMap<CreateJobDto, Job>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.JobType, opt => opt.Ignore())
                .ForMember(dest => dest.Sensor, opt => opt.Ignore())
                .ForMember(dest => dest.JobControlMappings, opt => opt.Ignore());

            // Update DTO to Entity (not commonly used, but included for completeness)
            CreateMap<UpdateJobDto, Job>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
