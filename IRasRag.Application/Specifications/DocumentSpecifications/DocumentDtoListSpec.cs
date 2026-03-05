using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.DocumentSpecifications
{
    public class DocumentDtoListSpec : BaseListSpec<Document, DocumentDto>
    {
        public DocumentDtoListSpec(DocumentListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<Document, object?>>>
            {
                ["uploadedat"] = d => d.UploadedAt,
                ["title"] = d => d.Title,
            };

            ApplySearch(request.SearchTerm, [d => d.Title]);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "uploadedat");

            Query.Select(d => new DocumentDto
            {
                Id = d.Id,
                Title = d.Title,
                FileUrl = d.FileUrl,
                UploadedByUserId = d.UploadedByUserId,
                UploadedByUserEmail = d.UploadedByUser.Email,
                UploadedAt = d.UploadedAt,
            });
        }
    }
}
