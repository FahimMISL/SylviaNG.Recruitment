using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateTagSuggestions
{
    public class CandidateTagSuggestionsHandler : IRequestHandler<CandidateTagSuggestionsQuery, List<string>>
    {
        private readonly ICandidateTagService _candidateTagService;

        public CandidateTagSuggestionsHandler(ICandidateTagService candidateTagService)
        {
            _candidateTagService = candidateTagService;
        }

        public async Task<List<string>> Handle(CandidateTagSuggestionsQuery query, CancellationToken cancellationToken)
        {
            return await _candidateTagService.GetSuggestionsAsync(query.Search);
        }
    }
}
