using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.DocumentSpecifications
{
    /// <summary>
    /// Specification chiếu một Document theo Id thành DocumentDto,
    /// bao gồm cả thông tin User (UploadedByUserEmail).
    /// </summary>
    public class DocumentDtoByIdSpec : Specification<Document, DocumentDto>
    {
        public DocumentDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(d => d.Id == id)
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
