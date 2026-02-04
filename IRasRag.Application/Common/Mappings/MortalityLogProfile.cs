using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class MortalityLogProfile : Profile
    {
        public MortalityLogProfile()
        {
            // Entity to DTO (for reading)
            CreateMap<MortalityLog, MortalityLogDto>();

            // Create DTO to Entity (for creation)
            CreateMap<CreateMortalityLogDto, MortalityLog>(MemberList.Source);

            // Update DTO to Entity (for updates)
            CreateMap<UpdateMortalityLogDto, MortalityLog>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
