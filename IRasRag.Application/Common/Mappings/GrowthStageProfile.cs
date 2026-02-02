using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class GrowthStageProfile : Profile
    {
        public GrowthStageProfile()
        {
            CreateMap<GrowthStage, GrowthStageDto>();

            CreateMap<CreateGrowthStageDto, GrowthStage>(MemberList.Source);

            CreateMap<UpdateGrowthStageDto, GrowthStage>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
