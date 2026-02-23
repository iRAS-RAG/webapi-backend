using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.CameraSpecifications
{
    public class CameraDtoListSpec : BaseListSpec<Camera, CameraDto>
    {
        public CameraDtoListSpec(CameraListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<Camera, object?>>>
            {
                ["createdat"] = c => c.CreatedAt ?? DateTime.MinValue,
                ["name"] = c => c.Name,
            };

            ApplySearch(
                request.SearchTerm,
                [
                    c => c.Name,
                ]
            );

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "createdat");

            Query.Select(c => new CameraDto
            {
                Id = c.Id,
                Name = c.Name,
                Url = c.Url,
                FarmId = c.FarmId,
                CreatedAt = c.CreatedAt,
                ModifiedAt = c.ModifiedAt,
            });
        }
    }
}
