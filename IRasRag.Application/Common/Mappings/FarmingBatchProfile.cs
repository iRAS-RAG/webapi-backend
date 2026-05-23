using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Common.Mappings
{
    public class FarmingBatchProfile : Profile
    {
        public FarmingBatchProfile()
        {
            // Entity to DTO
            CreateMap<FarmingBatch, FarmingBatchDto>()
                .ForMember(dest => dest.FishTankName, opt => opt.MapFrom(src => src.FishTank.Name))
                .ForMember(
                    dest => dest.SpeciesStageConfigId,
                    opt => opt.MapFrom(src => src.CurrentStageConfigId)
                )
                .ForMember(
                    dest => dest.SpeciesName,
                    opt => opt.MapFrom(src => src.CurrentStageConfig.Species.Name)
                )
                .ForMember(
                    dest => dest.StageName,
                    opt => opt.MapFrom(src => src.CurrentStageConfig.GrowthStage.Name)
                )
                .ForMember(dest => dest.PlannedStages, opt => opt.MapFrom(src => src.BatchStages));

            // Create DTO to Entity
            CreateMap<CreateFarmingBatchDto, FarmingBatch>(MemberList.None)
                .ForMember(dest => dest.CurrentStageConfigId, opt => opt.Ignore())
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => FarmingBatchStatus.ACTIVE)
                )
                .ForMember(
                    dest => dest.CurrentQuantity,
                    opt => opt.MapFrom(src => src.InitialQuantity)
                );

            // Update DTO to Entity
            CreateMap<UpdateFarmingBatchDto, FarmingBatch>(MemberList.Source)
                .ForMember(
                    dest => dest.CurrentStageConfigId,
                    opt => opt.MapFrom(src => src.SpeciesStageConfigId)
                )
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<BatchStage, PlannedStageDto>()
                .ForMember(
                    dest => dest.SpeciesStageConfigId,
                    opt => opt.MapFrom(src => src.SpeciesStageConfigId)
                )
                .ForMember(
                    dest => dest.GrowthStageId,
                    opt =>
                        opt.MapFrom(src =>
                            src.SpeciesStageConfig.GrowthStage != null
                            && src.SpeciesStageConfig.GrowthStage.Id != Guid.Empty
                                ? src.SpeciesStageConfig.GrowthStage.Id
                                : src.SpeciesStageConfig.GrowthStageId
                        )
                )
                .ForMember(
                    dest => dest.StageName,
                    opt =>
                        opt.MapFrom(src =>
                            src.SpeciesStageConfig.GrowthStage != null
                                ? src.SpeciesStageConfig.GrowthStage.Name
                                : string.Empty
                        )
                )
                .ForMember(
                    dest => dest.ExpectedDurationDays,
                    opt => opt.MapFrom(src => src.ExpectedDurationDays)
                )
                .ForMember(
                    dest => dest.EstimatedStartDate,
                    opt => opt.MapFrom(src => src.EstimatedStartDate)
                )
                .ForMember(
                    dest => dest.EstimatedEndDate,
                    opt => opt.MapFrom(src => src.EstimatedEndDate)
                );
            CreateMap<BatchStage, PlannedStageDto>()
                .ForMember(
                    dest => dest.ActualStartDate,
                    opt => opt.MapFrom(src => src.ActualStartDate)
                )
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(src => src.ActualEndDate))
                .ForMember(
                    dest => dest.AmountPer100Fish,
                    opt => opt.MapFrom(src => src.SpeciesStageConfig.AmountPer100Fish)
                )
                .ForMember(
                    dest => dest.FrequencyPerDay,
                    opt => opt.MapFrom(src => src.SpeciesStageConfig.FrequencyPerDay)
                )
                .ForMember(
                    dest => dest.MaxStockingDensity,
                    opt => opt.MapFrom(src => src.SpeciesStageConfig.MaxStockingDensity)
                )
                .ForMember(
                    dest => dest.FeedTypeNames,
                    opt =>
                        opt.MapFrom(src => src.SpeciesStageConfig.FeedTypes.Select(ft => ft.Name))
                );
        }
    }
}
