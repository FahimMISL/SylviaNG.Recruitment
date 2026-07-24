using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamSeatPlanDownloadExcel
{
    public class ExamSeatPlanDownloadExcelQuery : IRequest<ExamFileResponse>
    {
        public long ExamId { get; set; }

        public ExamSeatPlanDownloadExcelQuery(long examId)
        {
            ExamId = examId;
        }
    }
}
