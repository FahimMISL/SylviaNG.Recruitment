using MediatR;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Queries.CvBankCvBulkExportExcel
{
    public class CvBankCvBulkExportExcelQuery : IRequest<CvBankCvFileResponse>
    {
        public CvBankCvBulkRequest Request { get; }

        public CvBankCvBulkExportExcelQuery(CvBankCvBulkRequest request)
        {
            Request = request;
        }
    }
}
