using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class JobTypeProfile : Profile
    {
        public JobTypeProfile()
        {
            CreateMap<JobType, JobTypeDto>();
            CreateMap<CreateJobTypeDto, JobType>();
            CreateMap<UpdateJobTypeDto, JobType>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
