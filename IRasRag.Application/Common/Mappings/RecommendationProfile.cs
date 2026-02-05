using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class RecommendationProfile : Profile
    {
        public RecommendationProfile()
        {
            CreateMap<Recommendation, RecommendationDto>()
                .ForMember(
                    dest => dest.DocumentTitle,
                    opt => opt.MapFrom(src => src.Document.Title)
                );

            CreateMap<CreateRecommendationDto, Recommendation>(MemberList.Source);

            CreateMap<UpdateRecommendationDto, Recommendation>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
