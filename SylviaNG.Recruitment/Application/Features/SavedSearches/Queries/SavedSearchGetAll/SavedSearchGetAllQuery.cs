using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;

namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Queries.SavedSearchGetAll
{
    public class SavedSearchGetAllQuery : IRequest<List<SavedSearchResponse>>
    {
    }
}
