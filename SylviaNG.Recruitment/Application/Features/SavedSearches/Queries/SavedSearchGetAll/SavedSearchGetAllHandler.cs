using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Queries.SavedSearchGetAll
{
    public class SavedSearchGetAllHandler : IRequestHandler<SavedSearchGetAllQuery, List<SavedSearchResponse>>
    {
        private readonly ISavedSearchService _service;

        public SavedSearchGetAllHandler(ISavedSearchService service)
        {
            _service = service;
        }

        public async Task<List<SavedSearchResponse>> Handle(SavedSearchGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
