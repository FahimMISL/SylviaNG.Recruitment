using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Queries.SavedSearchGetById
{
    public class SavedSearchGetByIdHandler : IRequestHandler<SavedSearchGetByIdQuery, SavedSearchResponse>
    {
        private readonly ISavedSearchService _service;

        public SavedSearchGetByIdHandler(ISavedSearchService service)
        {
            _service = service;
        }

        public async Task<SavedSearchResponse> Handle(SavedSearchGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.SavedSearchId);
        }
    }
}
