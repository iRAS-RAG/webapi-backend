using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class MasterBoardProfile : Profile
    {
        public MasterBoardProfile()
        {
            CreateMap<MasterBoard, MasterBoardDto>()
                .ForMember(dest => dest.FishTankName, opt => opt.MapFrom(src => src.FishTank.Name));

            CreateMap<CreateMasterBoardDto, MasterBoard>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.FishTank, opt => opt.Ignore())
                .ForMember(dest => dest.Sensors, opt => opt.Ignore())
                .ForMember(dest => dest.ControlDevices, opt => opt.Ignore())
                .ValidateMemberList(MemberList.Source);

            CreateMap<UpdateMasterBoardDto, MasterBoard>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.FishTank, opt => opt.Ignore())
                .ForMember(dest => dest.Sensors, opt => opt.Ignore())
                .ForMember(dest => dest.ControlDevices, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
