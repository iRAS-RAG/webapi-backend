using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<PaginatedResult<DocumentDto>> GetAllDocumentsAsync(int page, int pageSize);
        Task<Result<DocumentDto>> GetDocumentByIdAsync(Guid id);
        Task<Result<DocumentDto>> CreateDocumentAsync(CreateDocumentDto createDto);
        Task<Result> UpdateDocumentAsync(Guid id, UpdateDocumentDto updateDto);
        Task<Result> DeleteDocumentAsync(Guid id);
    }
}
