using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class UserFarmProfile : Profile
    {
        public UserFarmProfile()
        {
            // Entity to DTO
            CreateMap<UserFarm, UserFarmDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(
                    dest => dest.UserFullName,
                    opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName)
                )
                .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Farm.Name));

            // Create DTO to Entity
            CreateMap<CreateUserFarmDto, UserFarm>(MemberList.Source);

            // Update DTO to Entity
            CreateMap<UpdateUserFarmDto, UserFarm>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
