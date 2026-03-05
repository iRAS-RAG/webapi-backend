using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<PaginatedResult<DocumentDto>> GetAllDocumentsAsync(DocumentListRequest request);
        Task<Result<DocumentDetailDto>> GetDocumentByIdAsync(Guid id);
        Task<Result> CreateDocumentAsync(CreateDocumentDto dto);
        Task<Result> UpdateDocumentAsync(Guid id, UpdateDocumentDto updateDto);
        Task<Result> DeleteDocumentAsync(Guid id);
    }
}
