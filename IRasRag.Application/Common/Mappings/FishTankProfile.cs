using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class FishTankProfile : Profile
    {
        public FishTankProfile()
        {
            CreateMap<FishTank, FishTankDto>()
                .ForMember(d => d.Height, opt => opt.MapFrom(s => s.Height))
                .ForMember(d => d.Radius, opt => opt.MapFrom(s => s.Radius))
                .ForMember(d => d.CurrentSpecies, opt => opt.Ignore())
                .ForMember(d => d.CurrentCount, opt => opt.Ignore())
                .ForMember(d => d.HasOpenAlert, opt => opt.Ignore());

            CreateMap<CreateFishTankDto, FishTank>(MemberList.Source);

            CreateMap<UpdateFishTankDto, FishTank>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
