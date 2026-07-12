using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateProfileGetMe
{
    public class CandidateProfileGetMeHandler : IRequestHandler<CandidateProfileGetMeQuery, CandidateProfileResponse>
    {
        private readonly ICandidateProfileService _candidateProfileService;

        public CandidateProfileGetMeHandler(ICandidateProfileService candidateProfileService)
        {
            _candidateProfileService = candidateProfileService;
        }

        public async Task<CandidateProfileResponse> Handle(CandidateProfileGetMeQuery query, CancellationToken cancellationToken)
        {
            return await _candidateProfileService.GetMyProfileAsync();
        }
    }
}
