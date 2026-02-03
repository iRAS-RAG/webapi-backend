using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class FishTankProfile : Profile
    {
        public FishTankProfile()
        {
            CreateMap<FishTank, FishTankDto>();

            CreateMap<CreateFishTankDto, FishTank>(MemberList.Source);

            CreateMap<UpdateFishTankDto, FishTank>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
