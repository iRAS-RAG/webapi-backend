using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class CorrectiveActionProfile : Profile
    {
        public CorrectiveActionProfile()
        {
            CreateMap<CorrectiveAction, CorrectiveActionDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));

            CreateMap<CreateCorrectiveActionDto, CorrectiveAction>(MemberList.Source)
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateCorrectiveActionDto, CorrectiveAction>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
