using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;

namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Queries.SavedSearchGetById
{
    public class SavedSearchGetByIdQuery : IRequest<SavedSearchResponse>
    {
        public long SavedSearchId { get; set; }

        public SavedSearchGetByIdQuery(long savedSearchId)
        {
            SavedSearchId = savedSearchId;
        }
    }
}
