using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class SpeciesThresholdProfile : Profile
    {
        public SpeciesThresholdProfile()
        {
            CreateMap<SpeciesThreshold, SpeciesThresholdDto>()
                .ForMember(dest => dest.UnitOfMeasure, opt => opt.MapFrom(src => src.SensorType != null ? src.SensorType.UnitOfMeasure : null));

            CreateMap<CreateSpeciesThresholdDto, SpeciesThreshold>(MemberList.Source);

            CreateMap<UpdateSpeciesThresholdDto, SpeciesThreshold>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
