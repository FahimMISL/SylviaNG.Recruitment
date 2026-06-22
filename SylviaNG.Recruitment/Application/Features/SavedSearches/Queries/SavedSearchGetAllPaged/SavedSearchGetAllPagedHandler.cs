using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Queries.SavedSearchGetAllPaged
{
    public class SavedSearchGetAllPagedHandler : IRequestHandler<SavedSearchGetAllPagedQuery, PagedResult<SavedSearchResponse>>
    {
        private readonly ISavedSearchService _savedSearchService;

        public SavedSearchGetAllPagedHandler(ISavedSearchService savedSearchService)
        {
            _savedSearchService = savedSearchService;
        }

        public async Task<PagedResult<SavedSearchResponse>> Handle(SavedSearchGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _savedSearchService.GetPaginatedAsync(query.Request);
        }
    }
}
