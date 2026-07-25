using MediatR;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Queries.CvBankCvBulkDownload
{
    public class CvBankCvBulkDownloadQuery : IRequest<CvBankCvFileResponse>
    {
        public CvBankCvBulkRequest Request { get; }

        public CvBankCvBulkDownloadQuery(CvBankCvBulkRequest request)
        {
            Request = request;
        }
    }
}
