using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Mappings
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<Document, DocumentDto>()
                .ForMember(
                    dest => dest.UploadedByUserEmail,
                    opt => opt.MapFrom(src => src.UploadedByUser.Email)
                );

            CreateMap<CreateDocumentDto, Document>(MemberList.Source)
                .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateDocumentDto, Document>(MemberList.Source)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
