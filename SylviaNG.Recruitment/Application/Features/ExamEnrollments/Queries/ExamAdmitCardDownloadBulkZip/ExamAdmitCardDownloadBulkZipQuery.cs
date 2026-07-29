using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamAdmitCardDownloadBulkZip
{
    public class ExamAdmitCardDownloadBulkZipQuery : IRequest<ExamFileResponse>
    {
        public long ExamId { get; set; }

        public ExamAdmitCardDownloadBulkZipQuery(long examId)
        {
            ExamId = examId;
        }
    }
}
