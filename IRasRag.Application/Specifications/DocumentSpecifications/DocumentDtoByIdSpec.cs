using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.DocumentSpecifications
{
    /// <summary>
    /// Specification chiếu một Document theo Id thành DocumentDto,
    /// bao gồm cả thông tin User (UploadedByUserEmail).
    /// </summary>
    public class DocumentDtoByIdSpec : Specification<Document, DocumentDetailDto>
    {
        public DocumentDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(d => d.Id == id)
                .Select(d => new DocumentDetailDto
                {
                    Id = d.Id,
                    Title = d.Title,
                    FileUrl = d.FileUrl,
                    UploadedByUserId = d.UploadedByUserId,
                    UploadedByUserEmail = d.UploadedByUser.Email,
                    UploadedAt = d.UploadedAt,
                    RagStatus = d.RagStatus,
                });
        }
    }
}
