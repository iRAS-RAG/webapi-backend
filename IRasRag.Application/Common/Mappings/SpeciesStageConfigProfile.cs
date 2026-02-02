using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class SpeciesStageConfigProfile : Profile
    {
        public SpeciesStageConfigProfile()
        {
            CreateMap<SpeciesStageConfig, SpeciesStageConfigDto>().ReverseMap();

            CreateMap<CreateSpeciesStageConfigDto, SpeciesStageConfig>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Species, opt => opt.Ignore())
                .ForMember(dest => dest.GrowthStage, opt => opt.Ignore())
                .ForMember(dest => dest.FeedType, opt => opt.Ignore());

            CreateMap<UpdateSpeciesStageConfigDto, SpeciesStageConfig>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Species, opt => opt.Ignore())
                .ForMember(dest => dest.GrowthStage, opt => opt.Ignore())
                .ForMember(dest => dest.FeedType, opt => opt.Ignore())
                .ForMember(dest => dest.SpeciesId, opt => opt.Ignore())
                .ForMember(dest => dest.GrowthStageId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
