using MediatR;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateTagSuggestions
{
    public class CandidateTagSuggestionsQuery : IRequest<List<string>>
    {
        public string? Search { get; set; }

        public CandidateTagSuggestionsQuery(string? search)
        {
            Search = search;
        }
    }
}
