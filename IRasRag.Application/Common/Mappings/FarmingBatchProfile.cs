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
                    opt => opt.MapFrom(src => src.SpeciesStageConfig.GrowthStageId)
                )
                .ForMember(
                    dest => dest.StageName,
                    opt => opt.MapFrom(src => src.SpeciesStageConfig.GrowthStage.Name)
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
        }
    }
}
