using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class SpeciesStageConfigProfile : Profile
    {
        public SpeciesStageConfigProfile()
        {
            CreateMap<SpeciesStageConfig, SpeciesStageConfigDto>()
                .ForMember(
                    dest => dest.FeedTypeIds,
                    opt => opt.MapFrom(src => src.FeedTypes.Select(ft => ft.Id))
                )
                .ForMember(
                    dest => dest.FeedTypeNames,
                    opt => opt.MapFrom(src => src.FeedTypes.Select(ft => ft.Name))
                );

            // map sequence as well
            CreateMap<SpeciesStageConfig, SpeciesStageConfigDto>()
                .ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Sequence));

            CreateMap<CreateSpeciesStageConfigDto, SpeciesStageConfig>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Species, opt => opt.Ignore())
                .ForMember(dest => dest.GrowthStage, opt => opt.Ignore())
                .ForMember(dest => dest.FeedTypes, opt => opt.Ignore())
                .ForMember(dest => dest.FarmingBatches, opt => opt.Ignore());

            CreateMap<CreateSpeciesStageConfigDto, SpeciesStageConfig>()
                .ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Sequence));

            CreateMap<UpdateSpeciesStageConfigDto, SpeciesStageConfig>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Species, opt => opt.Ignore())
                .ForMember(dest => dest.GrowthStage, opt => opt.Ignore())
                .ForMember(dest => dest.FeedTypes, opt => opt.Ignore())
                .ForMember(dest => dest.AmountPer100Fish, opt => opt.Ignore())
                .ForMember(dest => dest.FrequencyPerDay, opt => opt.Ignore())
                .ForMember(dest => dest.ExpectedWeightKgPerFish, opt => opt.Ignore())
                .ForMember(dest => dest.SurvivalRate, opt => opt.Ignore())
                .ForMember(dest => dest.Sequence, opt => opt.Ignore())
                .ForMember(dest => dest.SpeciesId, opt => opt.Ignore())
                .ForMember(dest => dest.GrowthStageId, opt => opt.Ignore())
                .ForMember(dest => dest.FarmingBatches, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
