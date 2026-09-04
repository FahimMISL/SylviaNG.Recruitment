using MediatR;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Queries.CvBankCvDownload
{
    public class CvBankCvDownloadQuery : IRequest<CvBankCvFileResponse>
    {
        public long CandidateProfileId { get; }

        public CvBankCvDownloadQuery(long candidateProfileId)
        {
            CandidateProfileId = candidateProfileId;
        }
    }
}
