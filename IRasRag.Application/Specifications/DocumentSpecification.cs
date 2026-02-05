using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public class DocumentDtoListSpec : Specification<Document, DocumentDto>
    {
        public DocumentDtoListSpec()
        {
            Query
                .AsNoTracking()
                .Include(d => d.UploadedByUser)
                .Select(d => new DocumentDto
                {
                    Id = d.Id,
                    Title = d.Title,
                    Content = d.Content,
                    UploadedByUserId = d.UploadedByUserId,
                    UploadedByUserEmail = d.UploadedByUser.Email,
                    UploadedAt = d.UploadedAt,
                });
        }
    }
}
