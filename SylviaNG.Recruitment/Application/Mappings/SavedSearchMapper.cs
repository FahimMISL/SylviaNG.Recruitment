using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for the SavedSearch entity.
    /// </summary>
    public static class SavedSearchMapper
    {
        public static SavedSearch ToEntity(this SavedSearchCreateRequest request, string ownerUserName)
        {
            return new SavedSearch
            {
                Name = request.Name,
                IsShared = request.IsShared,
                FilterJson = request.FilterJson,
                OwnerUserName = ownerUserName
            };
        }

        public static SavedSearchLookupResponse ToLookupResponse(this SavedSearch entity, string? currentUserName)
        {
            return new SavedSearchLookupResponse
            {
                SavedSearchId = entity.SavedSearchId,
                Name = entity.Name,
                IsShared = entity.IsShared,
                IsOwner = entity.OwnerUserName == currentUserName,
                FilterJson = entity.FilterJson
            };
        }
    }
}
