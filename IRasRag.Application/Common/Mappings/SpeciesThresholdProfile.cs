using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class SpeciesThresholdProfile : Profile
    {
        public SpeciesThresholdProfile()
        {
            CreateMap<SpeciesThreshold, SpeciesThresholdDto>();

            CreateMap<CreateSpeciesThresholdDto, SpeciesThreshold>(MemberList.Source);

            CreateMap<UpdateSpeciesThresholdDto, SpeciesThreshold>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
