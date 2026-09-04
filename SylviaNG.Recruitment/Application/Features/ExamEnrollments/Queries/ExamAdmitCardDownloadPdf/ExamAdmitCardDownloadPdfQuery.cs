using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamAdmitCardDownloadPdf
{
    public class ExamAdmitCardDownloadPdfQuery : IRequest<ExamFileResponse>
    {
        public long ExamEnrollmentId { get; set; }

        public ExamAdmitCardDownloadPdfQuery(long examEnrollmentId)
        {
            ExamEnrollmentId = examEnrollmentId;
        }
    }
}
