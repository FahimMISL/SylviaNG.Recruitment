using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateProfileGetById
{
    public class CandidateProfileGetByIdHandler : IRequestHandler<CandidateProfileGetByIdQuery, CandidateProfileDetailResponse>
    {
        private readonly ICandidateProfileService _candidateProfileService;

        public CandidateProfileGetByIdHandler(ICandidateProfileService candidateProfileService)
        {
            _candidateProfileService = candidateProfileService;
        }

        public async Task<CandidateProfileDetailResponse> Handle(CandidateProfileGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _candidateProfileService.GetProfileDetailAsync(query.CandidateProfileId);
        }
    }
}
