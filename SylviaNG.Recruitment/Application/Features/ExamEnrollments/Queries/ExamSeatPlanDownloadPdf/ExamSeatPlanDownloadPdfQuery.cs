using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamSeatPlanDownloadPdf
{
    public class ExamSeatPlanDownloadPdfQuery : IRequest<ExamFileResponse>
    {
        public long ExamId { get; set; }

        public ExamSeatPlanDownloadPdfQuery(long examId)
        {
            ExamId = examId;
        }
    }
}
