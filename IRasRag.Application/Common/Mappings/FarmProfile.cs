using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class FarmProfile : Profile
    {
        public FarmProfile()
        {
            CreateMap<Farm, FarmDto>();

            CreateMap<CreateFarmDto, Farm>(MemberList.Source);

            CreateMap<UpdateFarmDto, Farm>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
