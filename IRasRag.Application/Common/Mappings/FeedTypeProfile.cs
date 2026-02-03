using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class FeedTypeProfile : Profile
    {
        public FeedTypeProfile()
        {
            CreateMap<FeedType, FeedTypeDto>();

            CreateMap<CreateFeedTypeDto, FeedType>(MemberList.Source);

            CreateMap<UpdateFeedTypeDto, FeedType>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
