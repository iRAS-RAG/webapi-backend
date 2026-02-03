using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class SpeciesProfile : Profile
    {
        public SpeciesProfile()
        {
            CreateMap<Species, SpeciesDto>();

            CreateMap<CreateSpeciesDto, Species>(MemberList.Source);

            CreateMap<UpdateSpeciesDto, Species>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
