using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateProfileDownloadPdf
{
    public class CandidateProfileDownloadPdfHandler : IRequestHandler<CandidateProfileDownloadPdfQuery, CandidateProfileDownloadResponse>
    {
        private readonly ICandidateProfileService _candidateProfileService;

        public CandidateProfileDownloadPdfHandler(ICandidateProfileService candidateProfileService)
        {
            _candidateProfileService = candidateProfileService;
        }

        public async Task<CandidateProfileDownloadResponse> Handle(CandidateProfileDownloadPdfQuery query, CancellationToken cancellationToken)
        {
            return await _candidateProfileService.DownloadProfilePdfAsync(query.CandidateProfileId);
        }
    }
}
