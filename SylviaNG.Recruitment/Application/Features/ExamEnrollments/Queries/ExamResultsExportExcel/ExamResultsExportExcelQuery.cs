using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamResultsExportExcel
{
    public class ExamResultsExportExcelQuery : IRequest<ExamFileResponse>
    {
        public long ExamId { get; set; }

        public ExamResultsExportExcelQuery(long examId)
        {
            ExamId = examId;
        }
    }
}
