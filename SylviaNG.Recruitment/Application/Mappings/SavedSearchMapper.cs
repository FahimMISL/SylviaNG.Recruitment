using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class SavedSearchMapper
    {
        public static SavedSearch ToEntity(this SavedSearchCreateRequest request)
        {
            return new SavedSearch
            {
                CreatedByUserId = request.CreatedByUserId,
                SearchName = request.SearchName,
                QueryExpression = request.QueryExpression,
            };
        }

        public static void ApplyUpdate(this SavedSearch entity, SavedSearchUpdateRequest request)
        {
            if (request.CreatedByUserId.HasValue) entity.CreatedByUserId = request.CreatedByUserId.Value;
            if (request.SearchName is not null) entity.SearchName = request.SearchName;
            if (request.QueryExpression is not null) entity.QueryExpression = request.QueryExpression;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static SavedSearchResponse ToResponse(this SavedSearch entity)
        {
            return new SavedSearchResponse
            {
                SavedSearchId = entity.SavedSearchId,
                CreatedByUserId = entity.CreatedByUserId,
                SearchName = entity.SearchName,
                QueryExpression = entity.QueryExpression,
                IsActive = entity.IsActive,
            };
        }
    }
}
