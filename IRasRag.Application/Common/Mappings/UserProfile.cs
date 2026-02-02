using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<CreateUserDto, User>(MemberList.Source)
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Password is hashed in service

            CreateMap<UpdateUserDto, User>(MemberList.Source)
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Password is hashed in service
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
