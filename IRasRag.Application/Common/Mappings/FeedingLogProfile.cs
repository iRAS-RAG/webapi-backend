using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class FeedingLogProfile : Profile
    {
        public FeedingLogProfile()
        {
            // Entity to DTO (for reading)
            CreateMap<FeedingLog, FeedingLogDto>();

            // Create DTO to Entity (for creation)
            CreateMap<CreateFeedingLogDto, FeedingLog>(MemberList.Source);

            // Update DTO to Entity (for updates)
            CreateMap<UpdateFeedingLogDto, FeedingLog>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
