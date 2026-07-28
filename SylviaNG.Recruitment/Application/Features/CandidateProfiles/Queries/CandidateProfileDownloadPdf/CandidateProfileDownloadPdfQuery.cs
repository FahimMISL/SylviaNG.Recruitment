using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateProfileDownloadPdf
{
    public class CandidateProfileDownloadPdfQuery : IRequest<CandidateProfileDownloadResponse>
    {
        public long CandidateProfileId { get; }

        public CandidateProfileDownloadPdfQuery(long candidateProfileId)
        {
            CandidateProfileId = candidateProfileId;
        }
    }
}
