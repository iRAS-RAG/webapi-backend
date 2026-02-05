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
                .ForMember(dest => dest.SpeciesName, opt => opt.MapFrom(src => src.Species.Name));

            // Create DTO to Entity
            CreateMap<CreateFarmingBatchDto, FarmingBatch>(MemberList.Source)
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
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
