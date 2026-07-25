using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateTagGetAll
{
    public class CandidateTagGetAllHandler : IRequestHandler<CandidateTagGetAllQuery, List<CandidateTagResponse>>
    {
        private readonly ICandidateTagService _candidateTagService;

        public CandidateTagGetAllHandler(ICandidateTagService candidateTagService)
        {
            _candidateTagService = candidateTagService;
        }

        public async Task<List<CandidateTagResponse>> Handle(CandidateTagGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _candidateTagService.GetAllAsync(query.CandidateProfileId);
        }
    }
}
