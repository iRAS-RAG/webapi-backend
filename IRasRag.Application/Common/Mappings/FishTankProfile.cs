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
                .ForMember(
                    d => d.Volume,
                    opt => opt.MapFrom(s => Math.PI * s.Radius * s.Radius * s.Height)
                )
                .ForMember(d => d.CurrentSpecies, opt => opt.Ignore())
                .ForMember(d => d.CurrentCount, opt => opt.Ignore())
                .ForMember(d => d.HasOpenAlert, opt => opt.Ignore());

            CreateMap<CreateFishTankDto, FishTank>(MemberList.Source);

            CreateMap<UpdateFishTankDto, FishTank>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
